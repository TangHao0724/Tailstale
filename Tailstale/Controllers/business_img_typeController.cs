﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class business_img_typeController : Controller
    {
        private readonly TailstaleContext _context;

        public business_img_typeController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: business_img_type
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.business_img_types.Include(b => b.FK_business);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: business_img_type/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img_type = await _context.business_img_types
                .Include(b => b.FK_business)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business_img_type == null)
            {
                return NotFound();
            }

            return View(business_img_type);
        }

        // GET: business_img_type/Create
        public IActionResult Create()
        {
            ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: business_img_type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FK_business_id,typename,created_at")] business_img_type business_img_type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business_img_type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name", business_img_type.FK_business_id);
            return View(business_img_type);
        }

        // GET: business_img_type/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img_type = await _context.business_img_types.FindAsync(id);
            if (business_img_type == null)
            {
                return NotFound();
            }
            ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name", business_img_type.FK_business_id);
            return View(business_img_type);
        }

        // POST: business_img_type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FK_business_id,typename,created_at")] business_img_type business_img_type)
        {
            if (id != business_img_type.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business_img_type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!business_img_typeExists(business_img_type.ID))
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
            ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name", business_img_type.FK_business_id);
            return View(business_img_type);
        }

        // GET: business_img_type/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img_type = await _context.business_img_types
                .Include(b => b.FK_business)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business_img_type == null)
            {
                return NotFound();
            }

            return View(business_img_type);
        }

        // POST: business_img_type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business_img_type = await _context.business_img_types.FindAsync(id);
            if (business_img_type != null)
            {
                _context.business_img_types.Remove(business_img_type);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool business_img_typeExists(int id)
        {
            return _context.business_img_types.Any(e => e.ID == id);
        }
    }
}
