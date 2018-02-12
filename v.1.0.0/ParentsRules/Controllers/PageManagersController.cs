using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Data;
using ParentsRules.Models;
using System.IO;

namespace ParentsRules.Controllers
{
    public class PageManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PageManagersController(ApplicationDbContext context)
        {
            _context = context;
        }
        #region Select Getter Methods
        private List<SelectListItem> GetBackgroundImages()
        {
            List<PageManager> pageConfigs = new List<PageManager>();
            //List<ChoreTypes> retList =
            List<SelectListItem> retList = new List<SelectListItem>();
            pageConfigs.ForEach(delegate (PageManager config)
            {
                
                retList.Add(new SelectListItem() { Value = config.BackgroundImageFileName, Text = config.BackgroundImageName });
            });


            retList.Insert(0, new SelectListItem() { Value = "", Text = "Select a Background Image", Selected = true });
            retList.Insert(1, new SelectListItem() { Value = "-1", Text = "Select Default White Background", Selected = false });


            return retList;
        }
        #endregion
        // GET: PageManagers
        public async Task<IActionResult> Index()
        {
            List<PageManagerViewModel> pageMgrList = new List<PageManagerViewModel>();
            PageManagerViewModel pageMgrItem;
            await _context.PageManager.ForEachAsync(delegate (PageManager item)  {
                pageMgrItem = new PageManagerViewModel() { ID=item.ID, BackgroundImageFileName = item.BackgroundImageFileName, BackgroundImageFilePath = item.BackgroundImageFilePath, BackgroundImageName = item.BackgroundImageName };
                pageMgrList.Add(pageMgrItem);
            });
            return View(pageMgrList.ToList());
        }

        // GET: PageManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageManager = await _context.PageManager
                .SingleOrDefaultAsync(m => m.ID == id);
            if (pageManager == null)
            {
                return NotFound();
            }

            return View(pageManager);
        }

        // GET: PageManagers/Create
        public IActionResult Create()
        {
            ViewBag.BackgroundImageList = GetBackgroundImages();
            return View();
        }

        // POST: PageManagers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BackgroundImageName,BackgroundImageFileName,BackgroundImageFilePath,BackgroundImage")] PageManagerViewModel pageManager)
        {
            if (ModelState.IsValid)
            {
                /* Upload file to images bgpageimages folder. */
                
                if (pageManager.BackgroundImage != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/bgpageimages/",pageManager.BackgroundImage.FileName);
                    using (var stream = new FileStream(path, FileMode.Create)){
                        await pageManager.BackgroundImage.CopyToAsync(stream);
                    }
                
                    pageManager.BackgroundImageFileName = pageManager.BackgroundImage.FileName;
                    pageManager.BackgroundImageFilePath = path;
                }
                PageManager newItem = new PageManager() { BackgroundImageFileName = pageManager.BackgroundImage.FileName, BackgroundImageName = pageManager.BackgroundImageName, BackgroundImageFilePath = pageManager.BackgroundImageFilePath };
                _context.Add(newItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pageManager);
        }

        // GET: PageManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageManager = await _context.PageManager.SingleOrDefaultAsync(m => m.ID == id);
            if (pageManager == null)
            {
                return NotFound();
            }
            PageManagerViewModel pageManagerItem = new PageManagerViewModel() { ID = pageManager.ID, BackgroundImageFileName = pageManager.BackgroundImageFileName, BackgroundImageFilePath = pageManager.BackgroundImageFilePath, BackgroundImageName = pageManager.BackgroundImageName };
            return View(pageManagerItem);
        }

        // POST: PageManagers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BackgroundImageName,BackgroundImageFileName,BackgroundImageFilePath,BackgroundImage")] PageManagerViewModel pageManager)
        {
            if (id != pageManager.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PageManager currentItem = _context.PageManager.Where(a => a.ID == pageManager.ID).FirstOrDefault();
                  
                    // Upload the new image.
                    if (pageManager.BackgroundImage != null)
                    {
                        //Delete the previous file
                        System.IO.File.Delete(currentItem.BackgroundImageFilePath);
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/bgpageimages/", pageManager.BackgroundImage.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await pageManager.BackgroundImage.CopyToAsync(stream);
                        }
                        // Delete the previous file
                        currentItem.BackgroundImageFilePath = path;
                        currentItem.BackgroundImageFileName = pageManager.BackgroundImage.FileName;

                    }
                    currentItem.BackgroundImageName = pageManager.BackgroundImageName;
                    
                    _context.Update(currentItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageManagerExists(pageManager.ID))
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
            return View(pageManager);
        }

        // GET: PageManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pageManager = await _context.PageManager
                .SingleOrDefaultAsync(m => m.ID == id);
            string fileName = pageManager.BackgroundImageFileName;
            //See how many users are using this file name.
            List<ApplicationUser> users = _context.AccountUsers.Where(a => a.BGPageImage == fileName).ToList();
            ViewBag.NumberOfUsers = users.Count;
            if (pageManager == null)
            {
                return NotFound();
            }

            return View(pageManager);
        }

        // POST: PageManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pageManager = await _context.PageManager.SingleOrDefaultAsync(m => m.ID == id);
            
            _context.PageManager.Remove(pageManager);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageManagerExists(int id)
        {
            return _context.PageManager.Any(e => e.ID == id);
        }
    }
}
