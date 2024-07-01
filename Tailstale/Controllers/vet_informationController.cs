using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hospital_ViewModel;
using Tailstale.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tailstale.Controllers
{
    public class vet_informationController : Controller
    {
        private readonly TailstaleContext _context;

        public vet_informationController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: vet_information
        public async Task<IActionResult> Index()
        {
            try
            {
                var tailstaleContext = _context.vet_informations.Include(v => v.business).Include(v => v.department);

                var v_Infovm = from v_info in _context.vet_informations
                               join b_img_types in _context.business_img_types
                               on v_info.business_ID equals b_img_types.FK_business_id
                               join b_img in _context.business_imgs
                               on b_img_types.ID equals b_img.img_type_id
                               select new vet_information_ViewModel
                               {
                                   vet_ID = v_info.vet_ID,
                                   vet_name = v_info.vet_name,
                                   license_number = v_info.license_number,
                                   profile = v_info.profile,
                                   business_ID = v_info.business_ID,
                                   department_ID = v_info.department_ID,
                                   ID = b_img.ID,
                                   img_type_id = b_img_types.ID,
                                   URL = b_img.URL,
                                   name = b_img.name
                               };

                return View(v_Infovm);
            }
            catch (Exception ex)
            {
                // 記錄錯誤訊息
                // Log.Error(ex, "An error occurred while executing Index.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: vet_information/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vet_information = await _context.vet_informations
                .Include(v => v.business)
                .Include(v => v.department)
                .FirstOrDefaultAsync(m => m.vet_ID == id);
            if (vet_information == null)
            {
                return NotFound();
            }

            return View(vet_information);
        }

        // GET: vet_information/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name");
            return View();
        }

        // POST: vet_information/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("vet_ID,vet_name,business_ID,license_number,department_ID,profile")] vet_information vet_information)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vet_information);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", vet_information.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", vet_information.department_ID);
            return View(vet_information);
        }

        // GET: vet_information/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vet_information = await _context.vet_informations.FindAsync(id);
            if (vet_information == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", vet_information.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", vet_information.department_ID);
            return View(vet_information);
        }

        // POST: vet_information/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("vet_ID,vet_name,business_ID,license_number,department_ID,profile")] vet_information vet_information)
        {
            if (id != vet_information.vet_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vet_information);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!vet_informationExists(vet_information.vet_ID))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", vet_information.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", vet_information.department_ID);
            return View(vet_information);
        }

        // GET: vet_information/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vet_information = await _context.vet_informations
                .Include(v => v.business)
                .Include(v => v.department)
                .FirstOrDefaultAsync(m => m.vet_ID == id);
            if (vet_information == null)
            {
                return NotFound();
            }

            return View(vet_information);
        }

        // POST: vet_information/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vet_information = await _context.vet_informations.FindAsync(id);
            if (vet_information != null)
            {
                _context.vet_informations.Remove(vet_information);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool vet_informationExists(int id)
        {
            return _context.vet_informations.Any(e => e.vet_ID == id);
        }
    }
}
