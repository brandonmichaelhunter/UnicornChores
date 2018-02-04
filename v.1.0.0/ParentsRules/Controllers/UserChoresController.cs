using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models;
using ParentsRules.Models.Chroes;
using ParentsRules.Models.Rooms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ParentsRules.Services;

namespace ParentsRules.Controllers
{
    
    [Authorize]
    public class UserChoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public UserChoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        [TempData]
        public string ErrorMessage { get; set; }
       
        #region SelectListGenerators
        private List<SelectListItem> GetChoreTypes(int SetSelecteChoreID = -1)
        {
            List<ChoreTypes> chores = _context.ChoreTypes.ToList<ChoreTypes>();
            //List<ChoreTypes> retList =
            List<SelectListItem> retList = new List<SelectListItem>();
            chores.ForEach(delegate (ChoreTypes chore)
            {
                if(SetSelecteChoreID != -1)
                {
                    if(chore.ID == SetSelecteChoreID)
                    {
                        retList.Add(new SelectListItem() { Value = chore.ID.ToString(), Text = chore.Chore, Selected=true });
                    }
                    else
                    {
                        retList.Add(new SelectListItem() { Value = chore.ID.ToString(), Text = chore.Chore });
                    }
                }
                else
                {
                    retList.Add(new SelectListItem() { Value = chore.ID.ToString(), Text = chore.Chore });
                }
                
            });

            if(SetSelecteChoreID == -1)
                retList.Insert(0, new SelectListItem() { Value = "", Text = "Select a Chore", Selected = true });


            return retList;
        }
        
        private List<SelectListItem> GetKids(string UserID, string SetSelectedChildID = null)
        {
            //Get a list of kids that is associated with the current user.
            //TODO - Get kids created by the current user and their friends as well.
            /* Get the id of parents related to the current user. */
            //List<string> parentIDs = _context.AccountAssociations.Where(a => a.PrimaryUserID == UserID && a.IsChild == false).Select(b => b.AssociatedUserID).ToList<string>();
            //parentIDs.Add(UserID);

            ///* Find all the children that each parent added to the database. */
            //List<AccountAssociations> children = new List<AccountAssociations>();
            //foreach (string parentID in parentIDs){
            //    children.AddRange(_context.AccountAssociations.Where(c => c.PrimaryUserID == parentID && c.IsChild == true).ToList<AccountAssociations>());
            //}


            //// Retrieve each child profile information.
            //List<ApplicationUser> childProfiles = new List<ApplicationUser>();
            //List<AccountAssociations> uniqueChildrenList = (from c in children select c).Distinct().ToList();
            //foreach (AccountAssociations child in uniqueChildrenList)
            //{
            //    var childProfile = _context.AccountUsers.Where(b => b.Id == child.AssociatedUserID).FirstOrDefault();
            //    if (childProfile != null)
            //    {
            //        childProfiles.Add(childProfile);
            //    }
            //}
            //Get the children for the current user.
            List<string> childrenIDs = _context.AccountAssociations.Where(a => a.PrimaryUserID == UserID && a.IsChild == true).Select(b => b.AssociatedUserID).Distinct().ToList<string>();
            List<ApplicationUser> childrenProfiles = new List<ApplicationUser>();
            ApplicationUser child;
            foreach (string childID in childrenIDs)
            {
                child = _context.AccountUsers.Where(c => c.Id == childID).FirstOrDefault();
                if (child != null)
                {
                    childrenProfiles.Add(child);
                }
            }

            // Get children for the current user related family members.
            List<ApplicationUser> familyMembersChildrenProfiles = new List<ApplicationUser>();

            List<string> familyMemberIDs = _context.AccountAssociations.Where(d => d.PrimaryUserID == UserID && d.IsChild == false).Select(e => e.AssociatedUserID).Distinct().ToList<string>();
            foreach (string familyMemberID in familyMemberIDs)
            {
                //Look up the children related to this family member.
                _context.AccountAssociations.Where(f => f.PrimaryUserID == familyMemberID && f.IsChild == true).Select(h => h.AssociatedUserID).Distinct().ToList<string>().ForEach(delegate (string childID) {
                    childrenProfiles.ForEach(delegate (ApplicationUser childProfile) {
                        if (childProfile.Id != childID)
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

            List<SelectListItem> retSelectList = new List<SelectListItem>();

            children.ForEach(delegate (ApplicationUser childrec)
            {
                if(SetSelectedChildID != null)
                {
                    if (childrec.Id == SetSelectedChildID)
                    {
                        retSelectList.Add(new SelectListItem() { Value = childrec.Id, Text = string.Format("{0} {1} {2}", childrec.FirstName, childrec.MiddleName, childrec.LastName), Selected=true });
                    }
                    else
                    {
                        retSelectList.Add(new SelectListItem() { Value = childrec.Id, Text = string.Format("{0} {1} {2}", childrec.FirstName, childrec.MiddleName, childrec.LastName) });
                    }
                }
                else
                {
                    retSelectList.Add(new SelectListItem() { Value = childrec.Id, Text = string.Format("{0} {1} {2}", childrec.FirstName, childrec.MiddleName, childrec.LastName) });
                }
                
            });
            if (SetSelectedChildID == null)
            {
                retSelectList.Add(new SelectListItem() { Value = "", Text = "Select a Child", Selected = true });
            }

            
            return retSelectList;
        }
        #endregion
        // GET: UserChores
        public async Task<IActionResult> Index()
        {

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                return View(await _context.UserChores.Where(a => a.UserID == user.Id).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }





        // GET: UserChores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userChores = await _context.UserChores
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (userChores == null)
                {
                    return NotFound();
                }

                return View(userChores);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: UserChores/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get the user's children
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                ViewBag.Children = GetKids(user.Id);

                //Get a list user created chores
                ViewBag.Chores = GetChoreTypes();

                
                

                //Get the current user ID value.
                ViewBag.CurrentUserID = user.Id;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        // POST: UserChores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ID,Chore,RoomID,UserID,ChoreDescription,DollarAmount,ParentID,DateDue,IsDaily,IsWeekly,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday")] UserChores userChores)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                userChores.ParentID = user.Id;
                userChores.PublishStatus = "Not Published";
                
                _context.Add(userChores);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }

        // GET: UserChores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userChores = await _context.UserChores.SingleOrDefaultAsync(m => m.ID == id);
                if (userChores == null)
                {
                    return NotFound();
                }
                // Get the user's children
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                ViewBag.Children = GetKids(user.Id, userChores.UserID);

                //Get a list user created chores
                ViewBag.Chores = GetChoreTypes(Convert.ToInt16(userChores.Chore));


                //Get the current user ID value.
                ViewBag.CurrentUserID = user.Id;

                if (userChores.DateDue.HasValue)
                {
                    if (userChores.DateDue.Value.Year == 1)
                    {
                        userChores.DateDue = null;
                    }
                }
                return View(userChores);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: UserChores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Chore,RoomID,ChoreDescription,UserID,DollarAmount,ParentID,DateDue,IsDaily,IsWeekly,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday")] UserChores userChores)
        {
            try
            {
                if (id != userChores.ID)
                {
                    return NotFound();
                }


                try
                {
                    
                    userChores.PublishStatus = "Not Published";
                    _context.Update(userChores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserChoresExists(userChores.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Dashboard");
               

                // Get the user's children
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                ViewBag.Children = GetKids(user.Id, userChores.UserID);

                //Get a list user created chores
                ViewBag.Chores = GetChoreTypes(Convert.ToInt16(userChores.Chore));


                //Get the current user ID value.
                ViewBag.CurrentUserID = user.Id;
                return View(userChores);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: UserChores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userChores = await _context.UserChores
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (userChores == null)
                {
                    return NotFound();
                }

                return View(userChores);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: UserChores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userChores = await _context.UserChores.SingleOrDefaultAsync(m => m.ID == id);
                _context.UserChores.Remove(userChores);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        private bool UserChoresExists(int id)
        {
            return _context.UserChores.Any(e => e.ID == id);
        }

        #region ChildrenWork
        public async Task<IActionResult> PublishChore(int id)
        {
            try
            {
                //Grab an instance of the chore.
                var chore = _context.UserChores.Where(a => a.ID == id).FirstOrDefault();
                if (chore != null)
                {
                    //Check to see if this is a new record or an old record.
                    List<ChildrenWork> workitems = _context.ChildWorkList.Where(a => a.ChoreID == id).ToList();
                    if (workitems.Count != 0)
                    {
                        // Delete all the records
                        _context.ChildWorkList.RemoveRange(_context.ChildWorkList.Where(a => a.ChoreID == id).ToList<ChildrenWork>());
                        await _context.SaveChangesAsync();
                    }


                    //Enter each chore as a seperate record for each day.
                    if (chore.Monday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = true,
                            Tuesday = false,
                            Wednesday = false,
                            Thursday = false,
                            Friday = false,
                            Saturday = false,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Tuesday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = true,
                            Wednesday = false,
                            Thursday = false,
                            Friday = false,
                            Saturday = false,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Wednesday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = false,
                            Wednesday = true,
                            Thursday = false,
                            Friday = false,
                            Saturday = false,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Thursday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = false,
                            Wednesday = false,
                            Thursday = true,
                            Friday = false,
                            Saturday = false,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Friday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = false,
                            Wednesday = false,
                            Thursday = false,
                            Friday = true,
                            Saturday = false,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Saturday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = false,
                            Wednesday = false,
                            Thursday = false,
                            Friday = false,
                            Saturday = true,
                            Sunday = false,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }
                    if (chore.Sunday)
                    {
                        var workitem = new ChildrenWork()
                        {
                            Chore = chore.Chore,
                            ChoreID = id,
                            RoomID = chore.RoomID,
                            UserID = chore.UserID,
                            DollarAmount = chore.DollarAmount,
                            ParentID = chore.ParentID,
                            DateDue = chore.DateDue,
                            Monday = false,
                            Tuesday = false,
                            Wednesday = false,
                            Thursday = false,
                            Friday = false,
                            Saturday = false,
                            Sunday = true,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.ChildWorkList.Add(workitem);
                    }



                    await _context.SaveChangesAsync();

                    //Update the Publish Status
                    chore.PublishStatus = "Published";
                    chore.LastPublishDate = DateTime.Now;
                    _context.UserChores.Update(chore);
                    await _context.SaveChangesAsync();
                }
                else
                {

                    _logger.LogError(string.Format("PublishCore - unable to publish chore id {0}", id));
                    throw new ApplicationException($"Unable to load publish chore.");

                }
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        #endregion
    }
}
