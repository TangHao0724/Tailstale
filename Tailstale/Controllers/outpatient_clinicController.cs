using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class outpatient_clinicController : Controller
    {
        private readonly TailstaleContext _context;

        public outpatient_clinicController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: outpatient_clinic
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.outpatient_clinics.Include(o => o.business).Include(o => o.department).Include(o => o.vet);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: outpatient_clinic/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outpatient_clinic = await _context.outpatient_clinics
                .Include(o => o.business)
                .Include(o => o.department)
                .Include(o => o.vet)
                .FirstOrDefaultAsync(m => m.outpatient_clinic_ID == id);
            if (outpatient_clinic == null)
            {
                return NotFound();
            }

            return View(outpatient_clinic);
        }

        // GET: outpatient_clinic/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name");
            ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_ID");
            return View();
        }

        // POST: outpatient_clinic/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("outpatient_clinic_ID,name,business_ID,department_ID,vet_ID,date,time,max_patients")] outpatient_clinic outpatient_clinic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(outpatient_clinic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", outpatient_clinic.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", outpatient_clinic.department_ID);
            ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_ID", outpatient_clinic.vet_ID);
            return View(outpatient_clinic);
        }

        // GET: outpatient_clinic/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outpatient_clinic = await _context.outpatient_clinics.FindAsync(id);
            if (outpatient_clinic == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", outpatient_clinic.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", outpatient_clinic.department_ID);
            ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_ID", outpatient_clinic.vet_ID);
            return View(outpatient_clinic);
        }

        // POST: outpatient_clinic/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("outpatient_clinic_ID,name,business_ID,department_ID,vet_ID,date,time,max_patients")] outpatient_clinic outpatient_clinic)
        {
            if (id != outpatient_clinic.outpatient_clinic_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(outpatient_clinic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!outpatient_clinicExists(outpatient_clinic.outpatient_clinic_ID))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", outpatient_clinic.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", outpatient_clinic.department_ID);
            ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_ID", outpatient_clinic.vet_ID);
            return View(outpatient_clinic);
        }

        // GET: outpatient_clinic/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outpatient_clinic = await _context.outpatient_clinics
                .Include(o => o.business)
                .Include(o => o.department)
                .Include(o => o.vet)
                .FirstOrDefaultAsync(m => m.outpatient_clinic_ID == id);
            if (outpatient_clinic == null)
            {
                return NotFound();
            }

            return View(outpatient_clinic);
        }

        // POST: outpatient_clinic/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var outpatient_clinic = await _context.outpatient_clinics.FindAsync(id);
            if (outpatient_clinic != null)
            {
                _context.outpatient_clinics.Remove(outpatient_clinic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool outpatient_clinicExists(int id)
        {
            return _context.outpatient_clinics.Any(e => e.outpatient_clinic_ID == id);
        }
    }
}
