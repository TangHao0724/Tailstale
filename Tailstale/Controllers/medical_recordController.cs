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
    public class medical_recordController : Controller
    {
        private readonly TailstaleContext _context;

        public medical_recordController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: medical_record
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.medical_records.Include(m => m.biological_test).Include(m => m.outpatient_clinic).Include(m => m.pet);
            return View(tailstaleContext);
        }

        // GET: medical_record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_record = await _context.medical_records
                .Include(m => m.biological_test)
                .Include(m => m.outpatient_clinic)
                .Include(m => m.pet)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medical_record == null)
            {
                return NotFound();
            }

            return View(medical_record);
        }

        // GET: medical_record/Create
        public IActionResult Create()
        {
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["outpatient_clinic_id"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "name");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID");
            return View();
        }

        // POST: medical_record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,pet_id,created_at,outpatient_clinic_id,weight,admission_process,diagnosis,treatment,biological_test_id,memo,fee")] medical_record medical_record)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medical_record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id", medical_record.biological_test_id);
            ViewData["outpatient_clinic_id"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "name", medical_record.outpatient_clinic_id);
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", medical_record.pet_id);
            return View(medical_record);
        }

        // GET: medical_record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_record = await _context.medical_records.FindAsync(id);
            if (medical_record == null)
            {
                return NotFound();
            }
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id", medical_record.biological_test_id);
            ViewData["outpatient_clinic_id"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "name", medical_record.outpatient_clinic_id);
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", medical_record.pet_id);
            return View(medical_record);
        }

        // POST: medical_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,pet_id,created_at,outpatient_clinic_id,weight,admission_process,diagnosis,treatment,biological_test_id,memo,fee")] medical_record medical_record)
        {
            if (id != medical_record.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medical_record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!medical_recordExists(medical_record.id))
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
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id", medical_record.biological_test_id);
            ViewData["outpatient_clinic_id"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "name", medical_record.outpatient_clinic_id);
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", medical_record.pet_id);
            return View(medical_record);
        }

        // GET: medical_record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_record = await _context.medical_records
                .Include(m => m.biological_test)
                .Include(m => m.outpatient_clinic)
                .Include(m => m.pet)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medical_record == null)
            {
                return NotFound();
            }

            return View(medical_record);
        }

        // POST: medical_record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medical_record = await _context.medical_records.FindAsync(id);
            if (medical_record != null)
            {
                _context.medical_records.Remove(medical_record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool medical_recordExists(int id)
        {
            return _context.medical_records.Any(e => e.id == id);
        }
    }
}
