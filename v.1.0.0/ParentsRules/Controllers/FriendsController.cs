using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ParentsRules.Models;
using Microsoft.Extensions.Logging;
using ParentsRules.Services;
using ParentsRules.Models.Chroes;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        public FriendsController(IEmailSender emailSender, ILogger<ManageController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        // GET: Friends
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                /* Query the user's friends. */
                ViewData["Friends"] = GetUserFriends(user.Id).AsEnumerable();

                /* Query the user's active friend requests. */
                ViewData["FriendRequests"] = GetUserFriendRequests(user.Id).AsEnumerable();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        public async Task<IActionResult> FriendRequests()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

               

                /* Query the user's active friend requests. */
                ViewData["FriendRequests"] = GetUserFriendRequests(user.Id).AsEnumerable();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }
        #region Children
        public async Task<IActionResult> Children()
        {
            try
            {
                //Get the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                //Get the children for the current user.
                List<string> childrenIDs = _context.AccountAssociations.Where(a => a.PrimaryUserID == user.Id && a.IsChild == true).Select(b => b.AssociatedUserID).Distinct().ToList<string>();
                List<ApplicationUser> childrenProfiles = new List<ApplicationUser>();
                ApplicationUser child;
                foreach (string childID in childrenIDs)
                {
                    child = _context.AccountUsers.Where(c => c.Id == childID).FirstOrDefault();
                    if(child != null)
                    {
                        childrenProfiles.Add(child);
                    }
                }

                // Get children for the current user related family members.
                List<ApplicationUser> familyMembersChildrenProfiles = new List<ApplicationUser>();
                
                List<string> familyMemberIDs = _context.AccountAssociations.Where(d => d.PrimaryUserID == user.Id && d.IsChild == false).Select(e => e.AssociatedUserID).Distinct().ToList<string>();
                foreach(string familyMemberID in familyMemberIDs)
                {
                    //Look up the children related to this family member.
                    _context.AccountAssociations.Where(f => f.PrimaryUserID == familyMemberID && f.IsChild == true).Select(h => h.AssociatedUserID).Distinct().ToList<string>().ForEach(delegate(string childID) {
                        childrenProfiles.ForEach(delegate (ApplicationUser childProfile) {
                            if(childProfile.Id != childID)
                            {
                                //Add the new child
                                child = _context.AccountUsers.Where(c => c.Id == childID).FirstOrDefault();
                                if (child != null)
                                {
                                    familyMembersChildrenProfiles.Add(child);
                                }
                            }
                        });
                    });
                }

                childrenProfiles.AddRange(familyMembersChildrenProfiles);
                List<ApplicationUser> children = (from c in childrenProfiles select c).Distinct().ToList();
                //Load the children profile records into the return view model
                List<ChildrenViewModel> childrenList = new List<ChildrenViewModel>();
  
                foreach (ApplicationUser childProfile in children)
                {

                    childrenList.Add(new ChildrenViewModel() { ID = childProfile.Id, FirstName = childProfile.FirstName, MiddleName = childProfile.MiddleName, LastName = childProfile.LastName, Username = childProfile.UserName });
                }
                


                return View(childrenList.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        

        }
        [HttpGet]
        public async Task<IActionResult> CreateChild()
        {
            /* Get the parent profile. */
            var parent = await _userManager.GetUserAsync(User);
            if (parent == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            ChildrenViewModel model = new ChildrenViewModel();
            model.Email = parent.Email;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChild(ChildrenViewModel model)
        {
            try
            {
                /* Get the parent profile. */
                var parent = await _userManager.GetUserAsync(User);
                if (parent == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                /* Create user account. */
                var child = new ApplicationUser { UserName = model.Username, Email = parent.Email, FirstName = model.FirstName, MiddleName = model.MiddleName, LastName = model.LastName, IsChild = "1" };
                var result = await _userManager.CreateAsync(child, model.Password);
                if (result.Succeeded)
                {
                    /* Automatically confirm the childs email.*/
                    child.EmailConfirmed = true;
                    _context.AccountUsers.Update(child);
                    _context.SaveChanges();

                    
                    /* Associate the child with their parent. */
                    var PrimaryUserAssociation = new AccountAssociations() { PrimaryUserID = parent.Id, AssociatedUserID = child.Id, IsChild = true };
                    _context.AccountAssociations.Add(PrimaryUserAssociation);
                   

                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Children));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        private List<string> GetRelatedParents(string CurrentUserID)
        {
            List<string> relatedParentIDs = _context.AccountAssociations.Where(a => a.PrimaryUserID == CurrentUserID && a.IsChild == false).Select(b => b.AssociatedUserID).ToList<string>();
            relatedParentIDs.Add(CurrentUserID);
            return relatedParentIDs;
        }

        // GET: Friends/Edit/5
        public async Task<IActionResult> EditChild(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //await _context.FriendViewModel.SingleOrDefaultAsync(m => m.ID == id);
            var rec = await _context.AccountUsers.SingleOrDefaultAsync(a => a.Id == id);
            ChildrenViewModel child;
            if (rec == null)
            {
                return NotFound();
            }
            else
            {
                child = new ChildrenViewModel()
                {
                    ID = rec.Id,
                    FirstName = rec.FirstName,
                    MiddleName = rec.MiddleName,
                    LastName = rec.LastName,
                    Username = rec.UserName,
                    Email = rec.Email
                };
            }
            
            return View(child);
        }

        // POST: Friends/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChild(string id, ChildrenViewModel childrenViewModel)
        {
            if (id != childrenViewModel.ID)
            {
                return NotFound();
            }

            
            try
            {
                var ChildUser = _context.AccountUsers.Where(a => a.Id == id).FirstOrDefault();
                ChildUser.FirstName = childrenViewModel.FirstName;
                ChildUser.MiddleName = childrenViewModel.MiddleName;
                ChildUser.LastName = childrenViewModel.LastName;
                ChildUser.UserName = childrenViewModel.Username;
                ChildUser.Email = childrenViewModel.Email;

                _context.Update(ChildUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                    return NotFound();
            }


            return RedirectToAction(nameof(Children));
        }
        // GET: Friends/Details/5
        public async Task<IActionResult> ChildDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendViewModel = await _context.FriendViewModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (friendViewModel == null)
            {
                return NotFound();
            }

            return View(friendViewModel);
        }
        // GET: Friends/Delete/5
        public async Task<IActionResult> DeleteChild(string id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                else
                {
                    //A child needs to be deleted from the following tables: AccountUsers, AccountAssociations, UserChores and ChildrenWork.
                    //Delete child's current tasks from the ChildrenWork table
                    List<ChildrenWork> worklist = _context.ChildWorkList.Where(a => a.UserID == id).ToList<ChildrenWork>();
                    if(worklist.Count > 0){
                        _context.ChildWorkList.RemoveRange(worklist);
                    }
                    //delete Child user chores
                    List<UserChores> choresList = _context.UserChores.Where(a => a.UserID == id).ToList<UserChores>();
                    if (choresList.Count > 0){
                        _context.UserChores.RemoveRange(choresList);
                    }
                    //Delete a child from account associations table
                    List<AccountAssociations> accountAssociationList = _context.AccountAssociations.Where(a => a.AssociatedUserID == id).ToList<AccountAssociations>();
                    if (accountAssociationList.Count > 0){
                        _context.AccountAssociations.RemoveRange(accountAssociationList);
                    }
                    //Find the record to delete
                    var child = _context.AccountUsers.Where(a => a.Id == id).FirstOrDefault();
                    if (child != null)
                    {
                        _context.AccountUsers.Remove(child);                       
                    }
                    else
                    {
                        throw new ApplicationException($"Child ID '{id}' does not exists.");
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Children));
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        // POST: Friends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChildConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                
                UserConformationRequests friendRequest = _context.UserConformationRequests.Where(a => a.RequestedUserID == user.Id && a.ID == id).FirstOrDefault();
                if (friendRequest != null)
                {
                    _context.UserConformationRequests.Remove(friendRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(FriendRequests));
                }
                else
                {
                    throw new ApplicationException($"Friend Request ID '{id}' does not exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }



        }
        #endregion
        private List<FriendViewModel> GetUserFriends(string id)
        {
            try
            {
                /* Query the AccountAssociations to find the out the friends of the current user.  */
                List<AccountAssociations> usersFriendsMap = _context.AccountAssociations.Where(a => a.PrimaryUserID == id && a.IsChild == false).ToList<AccountAssociations>();
                if (usersFriendsMap.Count > 0)
                {
                    List<FriendViewModel> userFriends = new List<FriendViewModel>();

                    // Process through the userFriendsMap list
                    foreach (AccountAssociations acct in usersFriendsMap)
                    {
                        var AccountUser = _context.AccountUsers.Where(a => a.Id == acct.AssociatedUserID).FirstOrDefault();
                        if (AccountUser != null)
                        {
                            var account = new FriendViewModel()
                            {
                                FirstName = AccountUser.FirstName,
                                MiddleName = AccountUser.MiddleName,
                                LastName = AccountUser.LastName,
                                Email = AccountUser.Email,
                                Username = AccountUser.UserName,
                                FriendSince = (_context.UserConformationRequests.Where(b => b.Email == AccountUser.Email && b.IsConfirmed == 1).First()).DateConfirmed
                            };
                            userFriends.Add(account);
                        }
                    }
                    return userFriends;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<FriendsRequestViewModel> GetUserFriendRequests(string id)
        {
            try
            {
                List<FriendsRequestViewModel> friendRequests = new List<FriendsRequestViewModel>();
                List<UserConformationRequests> requests = _context.UserConformationRequests.Where(a => a.RequestedUserID == id && a.IsConfirmed == 0).ToList<UserConformationRequests>();
                if (requests.Count > 0)
                {
                    foreach (UserConformationRequests request in requests)
                    {
                        var friendRequest = new FriendsRequestViewModel()
                        {
                            ID = request.ID,
                            DateRequested = request.DateSent,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Email = request.Email,
                            StatusMessage = StatusMessage
                        };
                        friendRequests.Add(friendRequest);
                    }
                    return friendRequests;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return null;
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequest(IndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                /* Send the friend request via email to your friend.*/
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.AssociatedUserLink(user.Id, code, Request.Scheme);
                var associatedUserEmail = model.Email;
                var associatedUserFirstName = model.AssociatedUserFirstName;
                var associatedUserLastName = model.AssociatedUserLastName;
                string websiteurl = Url.Action("Index", "Home");
                await _emailSender.SendParentRequestEmailConfirmationAsync(associatedUserEmail, callbackUrl, user.FirstName, associatedUserFirstName, websiteurl);
                /* Save the request to database */

                var request = new UserConformationRequests
                {
                    Email = associatedUserEmail,
                    DateSent = DateTime.Now,
                    IsConfirmed = 0,
                    ExpiredDate = DateTime.Now.AddDays(7),
                    RequestedUserID = user.Id,
                    FirstName = associatedUserFirstName,
                    LastName = associatedUserLastName,
                    RegistrationCode = code
                };
                _context.UserConformationRequests.Add(request);
                _context.SaveChanges();
                StatusMessage = "Parent link request sent. Please have then check their email your email.";
                return RedirectToAction(nameof(FriendRequests));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }
        // GET: Friends/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendViewModel = await _context.FriendViewModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (friendViewModel == null)
            {
                return NotFound();
            }

            return View(friendViewModel);
        }

        // GET: Friends/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Friends/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Username,Email,PhoneNumber,FriendSince")] FriendViewModel friendViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(friendViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(friendViewModel);
        }

        // GET: Friends/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendViewModel = await _context.FriendViewModel.SingleOrDefaultAsync(m => m.ID == id);
            if (friendViewModel == null)
            {
                return NotFound();
            }
            return View(friendViewModel);
        }

        // POST: Friends/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,MiddleName,LastName,Username,Email,PhoneNumber,FriendSince")] FriendViewModel friendViewModel)
        {
            if (id != friendViewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(friendViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FriendViewModelExists(friendViewModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(friendViewModel);
        }

        // GET: Friends/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                UserConformationRequests friendRequest = _context.UserConformationRequests.Where(a => a.RequestedUserID == user.Id && a.ID == id).FirstOrDefault();
                if (friendRequest != null)
                {
                    _context.UserConformationRequests.Remove(friendRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(FriendRequests));
                }
                else
                {
                    throw new ApplicationException($"Friend Request ID '{id}' does not exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        // POST: Friends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                UserConformationRequests friendRequest = _context.UserConformationRequests.Where(a => a.RequestedUserID == user.Id && a.ID == id).FirstOrDefault();
                if (friendRequest != null)
                {
                    _context.UserConformationRequests.Remove(friendRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(FriendRequests));
                }
                else
                {
                    throw new ApplicationException($"Friend Request ID '{id}' does not exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }



        }

        private bool FriendViewModelExists(int id)
        {
            return _context.FriendViewModel.Any(e => e.ID == id);
        }
    }
}
