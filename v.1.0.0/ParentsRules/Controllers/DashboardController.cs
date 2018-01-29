using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models.DashboardViewModels;
using Microsoft.AspNetCore.Identity;
using ParentsRules.Models;
using ParentsRules.Models.Chroes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ParentsRules.Services;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        private string GetChoreName(int ChoreID)
        {
            var chore = _context.ChoreTypes.Where(a => a.ID == ChoreID).FirstOrDefault();
            return (chore != null) ? chore.Chore : "N/A";
        }
      
        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                List<DashboardViewModel> dbItems = new List<DashboardViewModel>();
                //Get the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                else
                {
                    /* Get chores created by the current user only. */
                    List<UserChores> chores = new List<UserChores>();
                    chores = _context.UserChores.Where(a => a.ParentID == user.Id).OrderBy(b => b.UserID).ToList<UserChores>();

                    /* Get chores created by related users.*/
                    List<string> relatedParentIDs = _context.AccountAssociations.Where(b => b.PrimaryUserID == user.Id && b.IsChild == false).Select(c => c.AssociatedUserID).ToList<string>();

                    foreach(string relatedParentID in relatedParentIDs)
                    {
                        string UserID = relatedParentID;
                        //Look up chores created by the user.
                        var authorChores = _context.UserChores.Where(c => c.ParentID == UserID).OrderBy(d => d.UserID).ToList<UserChores>();
                        foreach(UserChores chore in authorChores)
                        {
                            //Check to see if the chore exists in the chores collection.
                            var currentChore = chores.Where(e => e.ID == chore.ID).FirstOrDefault();
                            if(currentChore == null)
                            {
                                chores.Add(currentChore);
                            }
                        }
                    }
                    /* Shows chores created by the current users and other parents. */
                    //List<AccountAssociations> UserFriends = _context.AccountAssociations.Where(a => a.PrimaryUserID == user.Id && a.IsChild == false).ToList<AccountAssociations>();
                    
                    //if (UserFriends.Count > 0)
                    //{
                    //    //Loop through the list and get chores created by the user and the user's friends.

                    //    //Get chores created by the current user.
                    //    chores = _context.UserChores.Where(a => a.ParentID == user.Id).OrderBy(b => b.UserID).ToList<UserChores>();

                    //    //Get chores created by the current user friends.
                    //    foreach (AccountAssociations friend in UserFriends)
                    //    {
                    //        if(friend.AssociatedUserID != user.Id)
                    //        {
                    //            //Get the user chores
                    //            List<UserChores> friendCreatedChores = _context.UserChores.Where(b => b.ParentID == friend.AssociatedUserID).ToList();
                    //            if(friendCreatedChores.Count > 0)
                    //            {
                    //                chores.AddRange(friendCreatedChores);
                    //            }
                    //        }
                    //    }
                    //}
                    DashboardViewModel dbItem;
                    if(chores.Count != 0)
                    {
                        string oldChildID = "";
                        string newChildID = "";
                        chores.ForEach(delegate (UserChores chore) {
                            oldChildID = chore.UserID;
                            if(oldChildID != newChildID)
                            {
                                newChildID = oldChildID;
                                dbItem = new DashboardViewModel();
                                dbItem.AssignedChildID = chore.UserID;
                                var cp = _context.AccountUsers.Where(c => c.Id == chore.UserID).FirstOrDefault();
                                if (cp != null)
                                {
                                    dbItem.AssignedChildName = string.Format("{0} {1} {2}", cp.FirstName, cp.MiddleName, cp.LastName);
                                }
                                dbItem.Chores = _context.UserChores.Where(d => d.UserID == chore.UserID).ToList<UserChores>();
                                dbItems.Add(dbItem);
                            }
                        
                        });
                    }
                }
                /* Retrieve the number of chores waiting to be review by parents */
                List<ChildrenWork> pendingCompletedChores =  GetPendingCompletedChores(user.Id);
                ViewData["PendingCompletedChores"] = pendingCompletedChores.Count;
                return View(dbItems.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        // GET: Dashboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var dashboardViewModel = await _context.DashboardViewModel
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (dashboardViewModel == null)
                {
                    return NotFound();
                }

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: Dashboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AssignedChildID,AssignedChildName")] DashboardViewModel dashboardViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(dashboardViewModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: Dashboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var dashboardViewModel = await _context.DashboardViewModel.SingleOrDefaultAsync(m => m.ID == id);
                if (dashboardViewModel == null)
                {
                    return NotFound();
                }
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        
            
        }

        // POST: Dashboard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AssignedChildID,AssignedChildName")] DashboardViewModel dashboardViewModel)
        {
            
            if (id != dashboardViewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dashboardViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DashboardViewModelExists(dashboardViewModel.ID))
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
            return View(dashboardViewModel);
        }

        // GET: Dashboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dashboardViewModel = await _context.DashboardViewModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (dashboardViewModel == null)
            {
                return NotFound();
            }

            return View(dashboardViewModel);
        }

        // POST: Dashboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dashboardViewModel = await _context.DashboardViewModel.SingleOrDefaultAsync(m => m.ID == id);
            _context.DashboardViewModel.Remove(dashboardViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DashboardViewModelExists(int id)
        {
            return _context.DashboardViewModel.Any(e => e.ID == id);
        }
        

        public async Task<IActionResult> ChoreCompleted(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                else if (user.IsChild == "1")
                {
                    throw new ApplicationException($"User '{user.FirstName}' tried to complete a chore on your behalf.");
                }
                else
                {
                    // Lookup the chore
                    ChildrenWork pendingchore = _context.ChildWorkList.Where(a => a.ID == id).FirstOrDefault();
                    if (pendingchore != null)
                    {
                        pendingchore.ParentVerified = true;
                        pendingchore.ParentVerifiedDate = DateTime.Now;
                        _context.ChildWorkList.Update(pendingchore);
                        CompletedChildrenWork completedChore = new CompletedChildrenWork() {
                            Chore = pendingchore.Chore,
                            RoomID = pendingchore.RoomID,
                            UserID = pendingchore.UserID,
                            DollarAmount = pendingchore.DollarAmount,
                            ParentID = pendingchore.ParentID,
                            DateDue = pendingchore.DateDue,
                            Monday = pendingchore.Monday,
                            MondayCompleted = pendingchore.MondayCompleted,
                            Tuesday = pendingchore.Tuesday,
                            TuesdayCompleted = pendingchore.TuesdayCompleted,
                            Wednesday = pendingchore.Wednesday,
                            WednesdayCompleted = pendingchore.WednesdayCompleted,
                            Thursday = pendingchore.Thursday,
                            ThursdayCompleted = pendingchore.ThursdayCompleted,
                            Friday = pendingchore.Friday,
                            FridayCompleted = pendingchore.FridayCompleted,
                            Saturday = pendingchore.Saturday,
                            SaturdayCompleted = pendingchore.SaturdayCompleted,
                            Sunday = pendingchore.Sunday,
                            SundayCompleted = pendingchore.SundayCompleted,
                            ParentVerified = pendingchore.ParentVerified,
                            ParentVerifiedDate = pendingchore.ParentVerifiedDate,
                            ChoreID = pendingchore.ChoreID,
                            TotalEarned = pendingchore.TotalEarned,
                            DateChoreCompleted = pendingchore.DateChoreCompleted,
                            ChoreCompleted = pendingchore.ChoreCompleted,
                            ChildCompleted = pendingchore.ChildCompleted,
                            StartOfWeekDate = UtilityService.GetFirstDayOfWeek(DateTime.Now),
                            StartOfWeekDateDisplay = UtilityService.GetFirstDayOfWeek(DateTime.Now).ToString("MM/dd/yyyy")
                        };
                        _context.CompletedChildWorkList.Add(completedChore);
                        //Save Completed Task to Archive table
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("CompletedChores", "Dashboard");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        public List<ChildrenWork> GetPendingCompletedChores(string UserID)
        {
            List<ChildrenWork> pendingChores = new List<ChildrenWork>();
            // Get the current user friends.
            List<string> parents = _context.AccountAssociations.Where(a => a.PrimaryUserID == UserID && a.IsChild == false).Select(b => b.AssociatedUserID).ToList<string>();
            parents.Add(UserID);
            if(parents.Count > 0){
               foreach(string parent in parents){
                    pendingChores.AddRange(_context.ChildWorkList.Where(c => c.ParentID == parent && c.ChoreCompleted == true && c.ParentVerified == false).OrderBy(d => d.UserID).OrderBy(e => e.DateChoreCompleted).ToList<ChildrenWork>());
               }
            }
            return pendingChores;
        }

        public async Task<IActionResult> CompletedChores() {
            try
            {
                //Get the current user
                List<ChildrenWork> completedChores = new List<ChildrenWork>();
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                else
                {
                    //Retrieve completed chores.

                    completedChores = GetPendingCompletedChores(user.Id);

                }

                return View(completedChores.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        public async Task<IActionResult> AllCompletedChores()
        {
            try
            {
                //Retrieve completed chores.
                List<CompletedChildrenWork> completedChores = new List<CompletedChildrenWork>();
                List<CompletedChildrenWorkViewModel> completedChoresViewList = new List<CompletedChildrenWorkViewModel>();
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                else
                {
                    
                    // Get the current user friends.
                    List<string> parents = _context.AccountAssociations.Where(a => a.PrimaryUserID == user.Id && a.IsChild == false).Select(b => b.AssociatedUserID).ToList<string>();
                    parents.Add(user.Id);
                    if (parents.Count > 0)
                    {
                        foreach (string parent in parents)
                        {
                            completedChores.AddRange(_context.CompletedChildWorkList.Where(c => c.ParentID == parent &&  c.ParentVerified == true).OrderBy(d => d.UserID).OrderByDescending(e => e.DateChoreCompleted).ToList<CompletedChildrenWork>());
                        }
                    }
                    // Build out Completed Chores dataset.
                    var startWeekDatesList = completedChores.GroupBy(b => b.StartOfWeekDateDisplay).ToList();
                    
                    
                    
                    
                    CompletedChildrenWorkViewModel rec;
                    int counterID = 0;
                    for(int f = 0; f <= startWeekDatesList.Count - 1; f++)
                    {
                        string startWeekDate = startWeekDatesList[f].Key;
                        
                        /* Search for chores where the StartOfWeekDateDisplay equals to startWeekDate */
                        List<CompletedChildrenWork> queryChores = completedChores.Where(a => a.StartOfWeekDateDisplay == startWeekDate).ToList();

                        // Get a unique list of children that has completed chores from the queryChores list.
                        List<string> childrenIDs = queryChores.Select(a => a.UserID).Distinct().ToList<string>();

                        // Get the child complete earnings and complete chores info.
                        List<CompletedChildWorkEarningsViewModel> childrenWorkWeekHistory = new List<CompletedChildWorkEarningsViewModel>();
                        int childCounterRecID = 0;
                        foreach (string childID in childrenIDs){
                            var childProfile = _context.AccountUsers.Where(a => a.Id == childID).FirstOrDefault();
                            string childDisplayName = string.Empty;
                            if (childProfile != null){
                                childDisplayName = string.Format("{0} {1} {2}", childProfile.FirstName, childProfile.MiddleName, childProfile.LastName);
                            }
                            float totalWeekEarnings = queryChores.Where(b => b.UserID == childID).Sum(c => c.DollarAmount);
                            List<CompletedChildrenWork> childCompletedChores = queryChores.Where(d => d.UserID == childID).ToList();
                            childrenWorkWeekHistory.Add(new CompletedChildWorkEarningsViewModel() { ChildID = childID, TotalWeekEarnings = totalWeekEarnings, ChildName = childDisplayName, CompletedChores = childCompletedChores });
                        }
                        
                        // Add unique ids
                        for (int x = 0; x <= childrenWorkWeekHistory.Count - 1; x++){
                            childCounterRecID = childCounterRecID + 1;
                            childrenWorkWeekHistory[x].ID = childCounterRecID;
                        }

                        counterID = counterID + 1;
                        rec = new CompletedChildrenWorkViewModel();
                        rec.ID = counterID;
                        rec.StartOfWeekDateDisplay = startWeekDate;
                        rec.TotalPayout = queryChores.Sum(e => e.DollarAmount);
                        rec.WeekWorkHistory = childrenWorkWeekHistory;
                        completedChoresViewList.Add(rec);
                    }

                    //Get Pending completed chores count
                    ViewData["PendingCompletedChores"] = GetPendingCompletedChores(user.Id).Count;
                    //Get Begining Week Day for chores
                    

                }

                return View(completedChoresViewList.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }
    }
}
