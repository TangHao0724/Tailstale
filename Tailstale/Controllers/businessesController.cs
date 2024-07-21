using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.Tools;

namespace Tailstale.Controllers
{
    public class businessesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TailstaleContext _context;

        public businessesController(TailstaleContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: businesses
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.businesses.Include(b => b.business_statusNavigation).Include(b => b.type);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: businessesnew/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.business_statusNavigation)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: businessesnew/Create
        public IActionResult Create()
        {
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name");
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name");
            return View();
        }
        // POST: businesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( business business,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {

                    //string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    string businessPath = Path.Combine(wwwRootPath, @"images\business");
                    using (var fileStream = new FileStream(Path.Combine(businessPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    business.photo_url = @"images\business\" + fileName;
                }
                _context.businesses.Add(business);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name", business.business_status);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            
            return View(business);
        }

        // GET: businesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }
            ViewData["business_statusNavigation"] = new SelectList(_context.member_statuses, "ID", "status_name", business.business_statusNavigation);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // POST: businesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  business business, IFormFile file)
        {
            if (id != business.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string businessPath=Path.Combine(wwwRootPath, @"images\business");
                    if (!string.IsNullOrEmpty(business.photo_url))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, business.photo_url.TrimStart('\\'));
                        if (!System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(businessPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    business.photo_url = @"images\business\" + fileName;
                }
                try
                {

                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!businessExists(business.ID))
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
            ViewData["business_statusNavigation"] = new SelectList(_context.member_statuses, "ID", "status_name", business.business_statusNavigation);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // GET: businesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.business_status)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: businesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business = await _context.businesses.FindAsync(id);
            if (business != null)
            {
                _context.businesses.Remove(business);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool businessExists(int id)
        {
            return _context.businesses.Any(e => e.ID == id);
        }

        
    }
}
