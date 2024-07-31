using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Tailstale.Controllers
{
    [IsLoginFilter]
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

                return PartialView("_hourPartial", new List<Business_hour>());
            }

            // 準備查詢
            IQueryable<Business_hour> query = _context.Business_hours
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

            var business_hour = await _context.Business_hours
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
        public async Task<IActionResult> Create([Bind("id,business_ID,business_day,open_time,close_time,people_limit")] Business_hourViewModel business_hour)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(business_hour);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //var businesses = _context.businesses
            //.Where(b => b.type_ID == 2)
            //.ToList();

            //ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ////ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            //return View(business_hour);
            if (ModelState.IsValid)
            {
                DateTime startDate = new DateTime(business_hour.business_day.Year, business_hour.business_day.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1); // 最後一天

                // 遍歷生成整個月份的營業時間資料
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    
                    
                        var businessHour = new Business_hour
                        {
                            business_ID = business_hour.business_ID,
                            business_day = DateOnly.FromDateTime(date.Date),
                            open_time = business_hour.open_time,//new TimeOnly(10, 0, 0),  // 預設開門時間，设置为早上10点
                            close_time = business_hour.close_time,//new TimeOnly(19, 0, 0),
                            people_limit = business_hour.people_limit
                        };
                    Business_hour a = new Business_hour()
                    {
                        business_ID = businessHour.business_ID,
                        business_day = businessHour.business_day,
                        open_time = businessHour.open_time,//new TimeOnly(10, 0, 0),  // 預設開門時間，设置为早上10点
                        close_time = businessHour.close_time,//new TimeOnly(19, 0, 0),
                        people_limit = businessHour.people_limit
                    };

                    //_context.Add(businessHour);
                    _context.Add(a);
                }
                

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 如果 ModelState 不合法，则需要重新加载视图
            var businesses = _context.businesses
                .Where(b => b.type_ID == 2)
                .ToList();

            ViewData["Business_ID"] = new SelectList(businesses, "ID", "Name", business_hour.business_ID);
            return View();
        }

        // GET: Business_hour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hours.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("id,business_ID,business_day,open_time,close_time,people_limit")] Business_hour business_hour)
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

            var business_hour = await _context.Business_hours
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
            var business_hour = await _context.Business_hours.FindAsync(id);
            if (business_hour != null)
            {
                _context.Business_hours.Remove(business_hour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Business_hourExists(int id)
        {
            return _context.Business_hours.Any(e => e.id == id);
        }
    }
}
