using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;
using ParentsRules.Models.Chroes;
using ParentsRules.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using ParentsRules.Services;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class ChildrenWorksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISMSSender _smsSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public ChildrenWorksController(ISMSSender smsSender, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _smsSender = smsSender;
        }

        // GET: ChildrenWorks
        
        
       
        public async Task<IActionResult> Index()
        {
            try
            {
                /* Get an instance of the logged in child user. */
                var user = await _userManager.GetUserAsync(User);
                if (user == null){
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                
                //Get the dollar amount earned so far.
                List<ChildrenWork> chorescompleted = _context.ChildWorkList.Where(a => a.UserID == user.Id && a.ChoreCompleted == true && a.ParentVerified == true).ToList<ChildrenWork>();
                double dollarsEarned = 0.00;
                if (chorescompleted.Count > 0){
                    foreach(ChildrenWork chore in chorescompleted){
                        dollarsEarned += chore.DollarAmount;
                    }
                    ViewData["DollarAmount"] = dollarsEarned;
                }

                string TodaysDayName = DateTime.Now.DayOfWeek.ToString();
                switch (TodaysDayName){
                    case "Monday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Monday == true).ToListAsync());

                    case "Tuesday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Tuesday == true).ToListAsync());

                    case "Wednesday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Wednesday == true).ToListAsync());

                    case "Thursday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Thursday == true).ToListAsync());

                    case "Friday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Friday == true).ToListAsync());

                    case "Saturday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Saturday == true).ToListAsync());

                    case "Sunday":
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Sunday == true).ToListAsync());

                    default:
                        return View(await _context.ChildWorkList.Where(a => a.UserID == user.Id && a.Monday == true).ToListAsync());
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }

        // GET: ChildrenWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var childrenWork = await _context.ChildWorkList
                .SingleOrDefaultAsync(m => m.ID == id);
            if (childrenWork == null)
            {
                return NotFound();
            }

            return View(childrenWork);
        }

        // GET: ChildrenWorks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChildrenWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Chore,RoomID,UserID,DollarAmount,ParentID,DateDue,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,ChoreCompleted,DateChoreCompleted,ParentVerified,ParentVerifiedDate,ChoreID")] ChildrenWork childrenWork)
        {
            try
            {

            
                if (ModelState.IsValid)
                {
                    _context.Add(childrenWork);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(childrenWork);
            }
            catch (Exception ex)
            {

                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        // GET: ChildrenWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var childrenWork = await _context.ChildWorkList.SingleOrDefaultAsync(m => m.ID == id);
            if (childrenWork == null)
            {
                return NotFound();
            }
            return View(childrenWork);
        }

        // POST: ChildrenWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Chore,RoomID,UserID,DollarAmount,ParentID,DateDue,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,ChoreCompleted,DateChoreCompleted,ParentVerified,ParentVerifiedDate,ChoreID")] ChildrenWork childrenWork)
        {
            try
            {
                if (id != childrenWork.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(childrenWork);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ChildrenWorkExists(childrenWork.ID))
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
                return View(childrenWork);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }

        // GET: ChildrenWorks/Delete/5        
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var childrenWork = await _context.ChildWorkList
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (childrenWork == null)
                {
                    return NotFound();
                }

                return View(childrenWork);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }

        // POST: ChildrenWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var childrenWork = await _context.ChildWorkList.SingleOrDefaultAsync(m => m.ID == id);
            _context.ChildWorkList.Remove(childrenWork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> ShowAllChores()
        {
            try
            {

            
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                //Need to group by chore id, and show non duplicates on this page.                
                List<ChoresViewModel> chores = new List<ChoresViewModel>();
                var uniqueChores = _context.ChildWorkList.Where(a => a.UserID == user.Id).OrderBy(b=>b.ChoreID).ToList<ChildrenWork>();
           
         

                foreach (var item in uniqueChores.Select(a=> a.ChoreID).Distinct())
                {
                    //lookup the chore by chore id.
                    List<ChildrenWork> lst = _context.ChildWorkList.Where(a => a.ChoreID == Convert.ToInt16(item) && a.UserID == user.Id).ToList<ChildrenWork>();
                    string allowence = "";
                    // Loop through each chore and save the days of work.
                    ChoresViewModel childchore = new ChoresViewModel();
                    foreach (var choreItem in lst)
                    {
                        allowence = choreItem.DollarAmount.ToString("C");
                        childchore.ID = choreItem.ID;
                        childchore.Chore = choreItem.Chore;
                        childchore.ChoreID = choreItem.ChoreID;
                        childchore.ChoreDescription = choreItem.ChoreDescription;
                        childchore.Monday = (choreItem.Monday) ? true : childchore.Monday;
                    
                        childchore.Tuesday = (choreItem.Tuesday) ? true : childchore.Tuesday;
                        childchore.Wednesday = (choreItem.Wednesday) ? true : childchore.Wednesday;
                        childchore.Thursday = (choreItem.Thursday) ? true : childchore.Thursday;
                        childchore.Friday = (choreItem.Friday) ? true : childchore.Friday;
                        childchore.Saturday = (choreItem.Saturday) ? true : childchore.Saturday;
                        childchore.Sunday = (choreItem.Sunday) ? true : childchore.Sunday;

                        childchore.MondayCompletedStatus = (choreItem.Monday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.MondayCompletedStatus;
                        childchore.TuesdayCompletedStatus = (choreItem.Tuesday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.TuesdayCompletedStatus;
                        childchore.WednesdayCompletedStatus = (choreItem.Wednesday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.WednesdayCompletedStatus;
                        childchore.ThursdayCompletedStatus = (choreItem.Thursday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.ThursdayCompletedStatus;
                        childchore.FridayCompletedStatus = (choreItem.Friday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.FridayCompletedStatus;
                        childchore.SaturdayCompletedStatus = (choreItem.Saturday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.SaturdayCompletedStatus;
                        childchore.SundayCompletedStatus = (choreItem.Sunday) ? GetCompletedStatus(choreItem.ChoreCompleted, choreItem.ParentVerified) : childchore.SundayCompletedStatus;
                    };
                    //set the allowence for each day
                    childchore.MondayAllowence = (childchore.Monday) ? allowence : "";
                    childchore.TuesdayAllowence = (childchore.Tuesday) ? allowence : "";
                    childchore.WednesdayAllowence = (childchore.Wednesday) ? allowence : "";
                    childchore.ThursdayAllowence = (childchore.Thursday) ? allowence : "";
                    childchore.FridayAllowence = (childchore.Friday) ? allowence : "";
                    childchore.SaturdayAllowence = (childchore.Saturday) ? allowence : "";
                    childchore.SundayAllowence = (childchore.Sunday) ? allowence : "";
                    chores.Add(childchore);
                }


                //Find the current amount of allowence collected so far 
                double dollarsEarned = 0.00;
                List<ChildrenWork> completedchores = _context.ChildWorkList.Where(a => a.UserID == user.Id && a.ParentVerified == true && a.ChoreCompleted == true).ToList<ChildrenWork>();
                if(completedchores.Count > 0){
                    foreach(var item in completedchores){
                        dollarsEarned += item.DollarAmount;
                    }
                    ViewData["DollarAmount"] = dollarsEarned;
                }

                return View(chores);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }

        private int GetCompletedStatus(bool choreCompleted, bool parentVerified)
        {
            if(choreCompleted && parentVerified)
            {
                return 2;
            }
            else if(choreCompleted && !parentVerified)
            {
                return 1;
            }
            else
            {
                return 0;
            }
            
        }

        private bool ChildrenWorkExists(int id)
        {
            return _context.ChildWorkList.Any(e => e.ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteChore(IFormCollection CompletedChores)
        {
            try
            {

                string completedchores = Convert.ToString(CompletedChores["CompletedChores"]);
                ViewBag.Results = completedchores;
                List<string> completedchoreList = new List<string>();
                if (string.IsNullOrEmpty(completedchores) == false){
                    IList<CompletedChoresList> completedchoreslist = JsonConvert.DeserializeObject<List<CompletedChoresList>>(completedchores);
                    //Update the Childrens work table.
                    foreach(CompletedChoresList item in completedchoreslist){
                        ChildrenWork chore = _context.ChildWorkList.Where(a => a.ID == item.TaskID).FirstOrDefault();
                        if(chore != null){

                            //update the chore
                            chore.MondayCompleted = (item.Monday == 1) ? true : false;
                            chore.TuesdayCompleted = (item.Tuesday == 1) ? true : false;
                            chore.WednesdayCompleted = (item.Wednesday == 1) ? true : false;
                            chore.ThursdayCompleted = (item.Thursday == 1) ? true : false;
                            chore.FridayCompleted = (item.Friday == 1) ? true : false;
                            chore.SaturdayCompleted = (item.Saturday == 1) ? true : false;
                            chore.SundayCompleted = (item.Sunday == 1) ? true : false;
                            chore.ChoreCompleted = true;
                            chore.DateChoreCompleted = DateTime.Now;
                            chore.ParentVerified = false;

                            _context.ChildWorkList.Update(chore);
                            await _context.SaveChangesAsync();
                            //Get the chore name
                            var chorename = _context.ChoreTypes.Where(b => b.ID == Convert.ToInt16(chore.Chore)).FirstOrDefault();
                            if(chorename != null)
                            {
                                completedchoreList.Add(chorename.Chore);
                            }
                        }
                    }
                }
 
                /* Get an instance of the logged in child user. */
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
               
                var childID = user.Id;
                string message = string.Format("Unicorn Chores: {0} has completed the following chores: {1}. Click on this link https://goo.gl/1kNbd6 to confirm that {0} chore(s) has been completed.", user.FirstName, String.Join(",", completedchoreList.ToArray<string>()));
                //Get parents ids as well.
                List<AccountAssociations> parentIDs = _context.AccountAssociations.Where(a => a.AssociatedUserID == childID).ToList<AccountAssociations>();
                if(parentIDs.Count != 0)
                {
                    List<string> phoneNumbers = new List<string>();
                    foreach(AccountAssociations parent in parentIDs)
                    {
                        //Get the parent profile record.
                        var parentProfile = _context.AccountUsers.Where(b => b.Id == parent.PrimaryUserID).FirstOrDefault();
                        if(parentProfile != null)
                        {
                            //Add the parent's phone number to the list.
                            phoneNumbers.Add(parentProfile.PhoneNumber);
                        }
                    }
                    //Notify the parents that a task has been completed.
                    await _smsSender.SendSMSMessage(message, phoneNumbers);
                }
                else
                {
                    throw new ApplicationException("There are not phone numbers to text.");
                }
                //ITwilioRestClient _client;
                //string _accountSid = "AC907f2ca57b80571d219274b8c056cc21";
                //string _authToken = "729d26355cd483620071a7d3bb56d501";
                //string twilioNumber = "+14126936790";

                //string message = string.Format("Unicorn Chores: {0} has completed the following chores {1}. Click on the https://tinyurl.com/1zq to access to the Unicron site and acess Completed Chores for further details.", user.FirstName, String.Join(",", completedchoreList.ToArray<string>()));
                //_client = new TwilioRestClient(_accountSid, _authToken);
                //var mommyPhoneNumber = new PhoneNumber(secondphonenumber);
                //var daddyPhoneNumber = new PhoneNumber("+17249141738");
                //MessageResource.Create(mommyPhoneNumber, from: new PhoneNumber(twilioNumber),body: message, client: _client);
                //MessageResource.Create(daddyPhoneNumber, from: new PhoneNumber(twilioNumber),body: message, client: _client);
            
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
        }
    }
}
