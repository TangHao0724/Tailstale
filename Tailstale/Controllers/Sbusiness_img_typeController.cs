using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsLoginFilter]
    public class Sbusiness_img_typeController : Controller
    {
        private readonly TailstaleContext _context;

        public Sbusiness_img_typeController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: business_img_type
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.business_img_types.Include(b => b.FK_business);
            var businesses3 = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();
            ViewData["FK_business_id"] = new SelectList(businesses3, "ID", "name");
            return View(await tailstaleContext.ToListAsync());
        }



        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {

            

            // 準備查詢
            IQueryable<business_img_type> query = _context.business_img_types
                .Include(bh => bh.FK_business);


            // 根據 id 的情況添加條件
            if (id.HasValue)
            {
                query = query.Where(bh => bh.FK_business_id == id);
            }

            

            // 执行查询并返回结果
            var S_img_type = await query.ToListAsync();

            return PartialView("_Sbusiness_img_typePartial", S_img_type);


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
            var businesses3 = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();
            ViewData["FK_business_id"] = new SelectList(businesses3, "ID", "name");
            //ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name");
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
            var businesses3 = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();
            ViewData["FK_business_id"] = new SelectList(businesses3, "ID", "name");
            
            //ViewData["FK_business_id"] = new SelectList(_context.businesses, "ID", "name", business_img_type.FK_business_id);
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
