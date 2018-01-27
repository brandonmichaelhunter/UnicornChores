using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models.Chroes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ParentsRules.Models;
using Microsoft.AspNetCore.Identity;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class ChoreTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChoreTypesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: ChoreTypes
        public async Task<IActionResult> Index()
        {
            try
            {
                List<ChoreTypes> choreTypes = new List<ChoreTypes>();
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                // Get chore types created by the current user and their friends.
                choreTypes.AddRange(_context.ChoreTypes.Where(a => a.AuthorUserID == user.Id).ToList());
                //Get the id's of the user friends.
                List<AccountAssociations> friends = _context.AccountAssociations.Where(b => b.PrimaryUserID == user.Id && b.IsChild == false).ToList();
                if(friends.Count > 0)
                {
                    foreach(AccountAssociations friend in friends)
                    {
                        choreTypes.AddRange(_context.ChoreTypes.Where(c => c.AuthorUserID == friend.AssociatedUserID).ToList());
                    }
                }
                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: ChoreTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var choreTypes = await _context.ChoreTypes
                    .SingleOrDefaultAsync(m => m.ID == id && m.AuthorUserID == user.Id);
                if (choreTypes == null)
                {
                    return NotFound();
                }

                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: ChoreTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChoreTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("ID,Chore,Description,IsActive")] ChoreTypes choreTypes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                    }
                    choreTypes.AuthorUserID = user.Id;
                    _context.Add(choreTypes);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: ChoreTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var choreTypes = await _context.ChoreTypes.SingleOrDefaultAsync(m => m.ID == id && m.AuthorUserID == user.Id);
                if (choreTypes == null)
                {
                    return NotFound();
                }
                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: ChoreTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Chore,Description,IsActive")] ChoreTypes choreTypes)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id != choreTypes.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        choreTypes.AuthorUserID = user.Id;
                        _context.Update(choreTypes);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ChoreTypesExists(choreTypes.ID))
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
                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: ChoreTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null)
                {
                    return NotFound();
                }

                var choreTypes = await _context.ChoreTypes.SingleOrDefaultAsync(m => m.ID == id && m.AuthorUserID == user.Id);
                if (choreTypes == null)
                {
                    return NotFound();
                }

                return View(choreTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: ChoreTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null){
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                //Before deleting the chore type, delete the record from the tables that reference the record.
                List<UserChores> userChores = _context.UserChores.Where(a => a.Chore == id.ToString()).ToList<UserChores>();
                if(userChores.Count > 0){
                    _context.UserChores.RemoveRange(userChores.ToArray());
                }

                List<ChildrenWork> activeChores = _context.ChildWorkList.Where(b => b.Chore == id.ToString()).ToList<ChildrenWork>();
                if(activeChores.Count > 0){
                    _context.ChildWorkList.RemoveRange(activeChores.ToArray());
                }

                var choreTypes = await _context.ChoreTypes.SingleOrDefaultAsync(m => m.ID == id && m.AuthorUserID == user.Id);
                _context.ChoreTypes.Remove(choreTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        private bool ChoreTypesExists(int id)
        {
            return _context.ChoreTypes.Any(e => e.ID == id);
        }
    }
}
