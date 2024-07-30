using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class ReserveController : Controller
    {
        private readonly TailstaleContext _context;

        public ReserveController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Reserves
        public async Task<IActionResult> Index()
        {
            //var tailstaleContext = _context.Reserve.Include(r => r.business).Include(r => r.keeper);
            //return View(await tailstaleContext.ToListAsync());
            var businesses = await _context.businesses
       .Where(b => b.type_ID == 2)
       .ToListAsync();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
       
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? id,string time)
        {
            //var tailstaleContext = _context.Business_hours.Include(b => b.business);
            //return View(await tailstaleContext.ToListAsync());

            //if (string.IsNullOrEmpty(time))
            //{
            //    ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            //    return PartialView("_ReservehourPartial", new List<Reserve>());
            //}


            if (!id.HasValue && string.IsNullOrEmpty(time))
            {
                
                return PartialView("_ReservehourPartial", new List<Reserve>());
            }

            // 準備查詢
            IQueryable<Reserve> query = _context.Reserves
                .Include(bh => bh.business)
                .Include(bh => bh.keeper)
            .Include(bh => bh.statusNavigation);

            // 根據 id 的情況添加條件
            if (id.HasValue)
            {
                query = query.Where(bh => bh.business_ID == id);
            }

            // 根據 time 的情況添加條件
            if (!string.IsNullOrEmpty(time))
            {
                // 將 time 字符串解析為 DateTime 對象
                DateTime selectedDate;
                if (!DateTime.TryParse(time, out selectedDate))
                {
                    // 如果解析失敗，返回空的部分視圖
                   
                    return PartialView("_ReservehourPartial", new List<Reserve>());
                }
                // 只比較日期部分
                query = query.Where(bh => bh.time.Date == selectedDate.Date);
            }

            // 执行查询并返回结果
            var businessHours = await query.ToListAsync();
            
            return PartialView("_ReservehourPartial", businessHours);




        }




    




        // GET: Reserve/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserves
                .Include(r => r.business)
                .Include(r => r.keeper)
                .Include(bh => bh.statusNavigation)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // GET: Reserve/Create
        public IActionResult Create()
        {
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();
            var orderstatus = _context.order_statuses
               .Where(b => b.business_type_ID == 2)
               .ToList();

            ViewData["Orderstatus_ID"] = new SelectList(orderstatus, "ID", "status_name");

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            return View();
        }

        // POST: Reserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,keeper_id,pet_name,business_ID,time,service_name,created_at,status")] Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserve);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            var orderstatus = _context.order_statuses
                .Where(b => b.business_type_ID == 2)
                .ToList();

            ViewData["Orderstatus_ID"] = new SelectList(orderstatus, "ID", "status_name");
            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", reserve.business_ID);
            //ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            return View(reserve);
        }

        // GET: Reserves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return NotFound();
            }
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            var pets = _context.pets
            .Where(p => p.keeper_ID == reserve.keeper_id) // 根据 keeper_id 进行过滤
           .ToList();

            var orderstatus = _context.order_statuses
               .Where(b => b.business_type_ID == 2)
               .ToList();

            ViewData["Orderstatus_ID"] = new SelectList(orderstatus, "ID", "status_name", reserve.status);

            ViewData["pet_name"] = new SelectList(pets, "name", "name");

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", reserve.business_ID);
            //ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            return View(reserve);
        }

        // POST: Reserves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,keeper_id,pet_name,business_ID,time,service_name,created_at,status")] Reserve reserve)
        {
            if (id != reserve.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserve);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReserveExists(reserve.id))
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
            var pets = _context.pets
            .Where(p => p.keeper_ID == reserve.keeper_id) // 根据 keeper_id 进行过滤
           .ToList();
            var orderstatus = _context.order_statuses
               .Where(b => b.business_type_ID == 2)
               .ToList();

            ViewData["Orderstatus_ID"] = new SelectList(orderstatus, "ID", "status_name", reserve.status);
            ViewData["pet_name"] = new SelectList(pets, "name", "name");
            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            //ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            return View(reserve);
        }

        // GET: Reserves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserves
                .Include(r => r.business)
                .Include(r => r.keeper)
                .Include(bh => bh.statusNavigation)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // POST: Reserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserve = await _context.Reserves.FindAsync(id);
            if (reserve != null)
            {
                _context.Reserves.Remove(reserve);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReserveExists(int id)
        {
            return _context.Reserves.Any(e => e.id == id);
        }
    }
}
