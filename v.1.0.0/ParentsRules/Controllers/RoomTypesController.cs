using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ParentsRules.Controllers
{
    [Authorize]
    public class RoomTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        public RoomTypesController(ApplicationDbContext context,ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: RoomTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.RoomTypes.ToListAsync());
        }

        // GET: RoomTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var roomTypes = await _context.RoomTypes
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (roomTypes == null)
                {
                    return NotFound();
                }

                return View(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: RoomTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoomTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Room,IsActive")] RoomTypes roomTypes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(roomTypes);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: RoomTypes/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var roomTypes = await _context.RoomTypes.SingleOrDefaultAsync(m => m.ID == id);
                if (roomTypes == null)
                {
                    return NotFound();
                }
                return View(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: RoomTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Room,IsActive")] RoomTypes roomTypes)
        {
            try
            {
                if (id != roomTypes.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(roomTypes);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RoomTypesExists(roomTypes.ID))
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
                return View(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // GET: RoomTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var roomTypes = await _context.RoomTypes
                    .SingleOrDefaultAsync(m => m.ID == id);
                if (roomTypes == null)
                {
                    return NotFound();
                }

                return View(roomTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var roomTypes = await _context.RoomTypes.SingleOrDefaultAsync(m => m.ID == id);
                _context.RoomTypes.Remove(roomTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        private bool RoomTypesExists(int id)
        {
            return _context.RoomTypes.Any(e => e.ID == id);
        }
    }
}
