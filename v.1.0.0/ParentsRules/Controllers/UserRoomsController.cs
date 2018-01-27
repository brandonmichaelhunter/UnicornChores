using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models.Rooms;
using ParentsRules.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class UserRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public UserRoomsController(ILogger<AccountController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: UserRooms
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            /* Get the current users account associtaions. */
            List<string> UserAccountAssociations = _context.AccountAssociations.Where(a => a.PrimaryUserID == user.Id && a.IsChild == false).Select(b=> b.AssociatedUserID).ToList();
            /* Return the user's friends rooms they created as well. */
            return View(await _context.UserRooms.Where(a=> UserAccountAssociations.Contains(a.UserID) || a.UserID == user.Id).ToListAsync());
        }
        public string DisplayName(string UserID)
        {
            string displayName = string.Empty;
            return displayName;
        }
        // GET: UserRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userRooms = await _context.UserRooms
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (userRooms == null)
                {
                    return NotFound();
                }

                return View(userRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        
        #region Create Actions
        // GET: UserRooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Room,UserID,DateCreated,IsActive")] UserRooms userRooms)
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
                    userRooms.UserID = user.Id;
                    userRooms.DateCreated = DateTime.Now;
                    userRooms.IsActive = true;
                    _context.Add(userRooms);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(userRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        #endregion

        #region Edit Actions
        // GET: UserRooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userRooms = await _context.UserRooms.SingleOrDefaultAsync(m => m.ID == id);
                if (userRooms == null)
                {
                    return NotFound();
                }
                return View(userRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: UserRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Room,UserID,DateCreated,IsActive")] UserRooms userRooms)
        {
            try
            {
                if (id != userRooms.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(userRooms);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserRoomsExists(userRooms.ID))
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
                return View(userRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: UserRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var userRooms = await _context.UserRooms
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (userRooms == null)
                {
                    return NotFound();
                }

                return View(userRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
           
        }
        #endregion
        
        #region Delete Actions
        // POST: UserRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userRooms = await _context.UserRooms.SingleOrDefaultAsync(m => m.ID == id);
                _context.UserRooms.Remove(userRooms);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        private bool UserRoomsExists(int id)
        {
            return _context.UserRooms.Any(e => e.ID == id);
        }
        #endregion
    }
}
