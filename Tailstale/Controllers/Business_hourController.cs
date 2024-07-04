using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Tailstale.Controllers
{
    public class Business_hourController : Controller
    {
        private readonly TailstaleContext _context;

        public Business_hourController(TailstaleContext context)
        {
            _context = context;
        }

        //GET: Business_hour 
        public async Task<IActionResult> Index()
        {
            var businesses = await _context.businesses
       .Where(b => b.type_ID == 2)
       .ToListAsync();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? id,string business_day)
        {
            
            if (!id.HasValue && string.IsNullOrEmpty(business_day))
            {

                return PartialView("_hourPartial", new List<Reserve>());
            }

            // 準備查詢
            IQueryable<Business_hour> query = _context.Business_hour
                .Include(bh => bh.business);
                

            // 根據 id 的情況添加條件
            if (id.HasValue)
            {
                query = query.Where(bh => bh.business_ID == id);
            }

            // 根據 time 的情況添加條件
            if (!string.IsNullOrEmpty(business_day))
            {
                // 將 time 字符串解析為 DateTime 對象
                DateOnly selectedDate;
                if (!DateOnly.TryParse(business_day, out selectedDate))
                {
                    // 如果解析失敗，返回空的部分視圖

                    return PartialView("_hourPartial", new List<Business_hour>());
                }
                // 只比較日期部分
                query = query.Where(bh => bh.business_day == selectedDate);
            }

            // 执行查询并返回结果
            var businessHours = await query.ToListAsync();

            return PartialView("_hourPartial", businessHours);


        }







        // GET: Business_hour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (business_hour == null)
            {
                return NotFound();
            }

            return View(business_hour);
        }

        // GET: Business_hour/Create
        public IActionResult Create()
        {
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Business_hour/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,business_ID,business_day,people_limit")] Business_hour business_hour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business_hour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // GET: Business_hour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour.FindAsync(id);
            if (business_hour == null)
            {
                return NotFound();
            }
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // POST: Business_hour/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,business_ID,business_day,people_limit")] Business_hour business_hour)
        {
            if (id != business_hour.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business_hour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Business_hourExists(business_hour.id))
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
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // GET: Business_hour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (business_hour == null)
            {
                return NotFound();
            }

            return View(business_hour);
        }

        // POST: Business_hour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business_hour = await _context.Business_hour.FindAsync(id);
            if (business_hour != null)
            {
                _context.Business_hour.Remove(business_hour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Business_hourExists(int id)
        {
            return _context.Business_hour.Any(e => e.id == id);
        }
    }
}
