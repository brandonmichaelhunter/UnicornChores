using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ParentsRules.Controllers
{
    [Produces("application/json")]
    [Route("api/ChildrenWorksAPI")]
    public class ChildrenWorksAPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        public ChildrenWorksAPIController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/ChildrenWorksAPI
        [HttpGet]
        public async Task<IEnumerable<ChildrenWork>> GetChildWorkList()
        {
            /* Get an instance of the logged in child user. */
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            /* Retrieve all tasks completed or not for the current week.*/
            List<ChildrenWork> worklist = _context.ChildWorkList.Where(a => a.UserID == user.Id).ToList<ChildrenWork>();
            return worklist.AsEnumerable<ChildrenWork>();
        }

        // GET: api/ChildrenWorksAPI/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChildrenWork([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var childrenWork = await _context.ChildWorkList.SingleOrDefaultAsync(m => m.ID == id);

            if (childrenWork == null)
            {
                return NotFound();
            }

            return Ok(childrenWork);
        }

        // PUT: api/ChildrenWorksAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChildrenWork([FromRoute] int id, [FromBody] ChildrenWork childrenWork)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != childrenWork.ID)
            {
                return BadRequest();
            }

            _context.Entry(childrenWork).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChildrenWorkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChildrenWorksAPI
        [HttpPost]
        public async Task<IActionResult> PostChildrenWork([FromBody] ChildrenWork childrenWork)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ChildWorkList.Add(childrenWork);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChildrenWork", new { id = childrenWork.ID }, childrenWork);
        }

        // DELETE: api/ChildrenWorksAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChildrenWork([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var childrenWork = await _context.ChildWorkList.SingleOrDefaultAsync(m => m.ID == id);
            if (childrenWork == null)
            {
                return NotFound();
            }

            _context.ChildWorkList.Remove(childrenWork);
            await _context.SaveChangesAsync();

            return Ok(childrenWork);
        }

        private bool ChildrenWorkExists(int id)
        {
            return _context.ChildWorkList.Any(e => e.ID == id);
        }
  
    }
}