using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Filter;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsHospitalFilter]
    public class departmentsController : Controller
    {
        private readonly TailstaleContext _context;

        public departmentsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: departments
        public async Task<IActionResult> Index()
        {
            int LoginID = (int)HttpContext.Session.GetInt32("loginID");
            var tailstaleContext = _context.departments.Where(d=>d.business_ID==LoginID);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: departments/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _context.departments
        //        .Include(d => d.business)
        //        .FirstOrDefaultAsync(m => m.department_ID == id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(department);
        //}

        // GET: departments/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("business_ID,department_name")] department department)
        { 
            int LoginID = (int)HttpContext.Session.GetInt32("loginID");
            if (LoginID == 0)
            { 
             return NotFound();
            }
            if (ModelState.IsValid)
                {
                    department.business_ID = LoginID;    
                    _context.Add(department);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", department.business_ID);
                return View(department);
        }

        // GET: departments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _context.departments.FindAsync(id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", department.business_ID);
        //    return View(department);
        //}

        // POST: departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("department_ID,business_ID,department_name")] department department)
        {
            TempData["ResultMessage"] = "edit_0";
            if (department.department_ID ==0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!departmentExists(department.department_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["ResultMessage"] = "edit_1";
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", department.business_ID);
            return View(department);
        }

        // GET: departments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _context.departments
        //        .Include(d => d.business)
        //        .FirstOrDefaultAsync(m => m.department_ID == id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(department);
        //}

        // POST: departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.departments.FindAsync(id);
            if (department != null)
            {
                _context.departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool departmentExists(int id)
        {
            return _context.departments.Any(e => e.department_ID == id);
        }
    }
}
