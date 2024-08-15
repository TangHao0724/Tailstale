using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Filter;
using Tailstale.Hospital_ViewModel;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsHospitalFilter]
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
            //var tailstaleContext = _context.outpatient_clinics.Include(o => o.outpatient_clinic_timeslot).Include(o => o.vet);
            var outpatientClinics = from opc in _context.outpatient_clinics
                                    join vInfo in _context.vet_informations
                                    on opc.vet_ID equals vInfo.vet_ID
                                   join opct in _context.outpatient_clinic_timeslots
                                   on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                   orderby opc.status, opc.dayofweek, opct.startat                               
                                   select new outpatient_clinic_ViewModel
                                   {
                                       outpatient_clinic_ID=opc.outpatient_clinic_ID,
                                       outpatient_clinic_name=opc.outpatient_clinic_name,
                                       outpatient_clinic_timeslot_ID=opc.outpatient_clinic_timeslot_ID,
                                       outpatient_clinic_timeslot_name=opct.outpatient_clinic_timeslot_name,
                                       startat=opct.startat,
                                       endat=opct.endat,
                                       dayofweek=opc.dayofweek,
                                       vet_ID=opc.vet_ID,
                                       vet_name=vInfo.vet_name,
                                       max_patients=opc.max_patients,
                                       status=opc.status,
                                   };
            ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_name");

            return View(outpatientClinics);
        }

        // GET: outpatient_clinic/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var outpatient_clinic = await _context.outpatient_clinics
        //        .Include(o => o.outpatient_clinic_timeslot)
        //        .Include(o => o.vet)
        //        .FirstOrDefaultAsync(m => m.outpatient_clinic_ID == id);
        //    if (outpatient_clinic == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(outpatient_clinic);
        //}

        // GET: outpatient_clinic/Create
        public IActionResult Create()
        {
            //ViewData["outpatient_clinic_timeslot_ID"] = new SelectList(_context.outpatient_clinic_timeslots, "outpatient_clinic_timeslot_ID", "outpatient_clinic_timeslot_name");
            ViewData["vet_ID"] = new SelectList(_context.vet_informations.Where(v => v.employment_status == true), "vet_ID", "vet_name");
            return View();
        }

        // POST: outpatient_clinic/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("outpatient_clinic_timeslot_name,startat,endat,dayofweek,outpatient_clinic_name,vet_ID,dayofweek,max_patients,status")] outpatient_clinic_ViewModel outpatientClinic)
        {
            if (ModelState.IsValid)
            {
                outpatient_clinic_timeslot opct = new outpatient_clinic_timeslot
                {
                    outpatient_clinic_timeslot_name = outpatientClinic.outpatient_clinic_timeslot_name,
                    startat = outpatientClinic.startat,
                    endat = outpatientClinic.endat,
                };
                _context.outpatient_clinic_timeslots.Add(opct);
                await _context.SaveChangesAsync();

                outpatient_clinic opc = new outpatient_clinic
                {
                    outpatient_clinic_name = outpatientClinic.outpatient_clinic_name,
                    outpatient_clinic_timeslot_ID = opct.outpatient_clinic_timeslot_ID,
                    vet_ID = outpatientClinic.vet_ID,                    
                    dayofweek = outpatientClinic.dayofweek,
                    max_patients = (int)outpatientClinic.max_patients,
                    status = outpatientClinic.status,
                };
                _context.outpatient_clinics.Add(opc);
                await _context.SaveChangesAsync();
            }
            else
            {                
                ViewData["vet_ID"] = new SelectList(_context.vet_informations.Where(v=>v.employment_status==true), "vet_ID", "vet_name", outpatientClinic.vet_ID);
                return View(outpatientClinic);

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: outpatient_clinic/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var outpatient_clinic = await _context.outpatient_clinics.FindAsync(id);
        //    if (outpatient_clinic == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["outpatient_clinic_timeslot_ID"] = new SelectList(_context.outpatient_clinic_timeslots, "outpatient_clinic_timeslot_ID", "outpatient_clinic_timeslot_name", outpatient_clinic.outpatient_clinic_timeslot_ID);
        //    ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_name", outpatient_clinic.vet_ID);
        //    return View(outpatient_clinic);
        //}

        // POST: outpatient_clinic/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("outpatient_clinic_ID,outpatient_clinic_name,vet_ID,max_patients,status")] edit_outpatient_clinic_ViewModel edit_outpatientClinic)
        {
            if (edit_outpatientClinic.outpatient_clinic_ID==0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var current_outpatientClinic = _context.outpatient_clinics.Find(edit_outpatientClinic.outpatient_clinic_ID);

                current_outpatientClinic.outpatient_clinic_ID = edit_outpatientClinic.outpatient_clinic_ID;
                current_outpatientClinic.outpatient_clinic_name = edit_outpatientClinic.outpatient_clinic_name;
                current_outpatientClinic.max_patients = (int)edit_outpatientClinic.max_patients;
                current_outpatientClinic.status = edit_outpatientClinic.status;
                
                try
                {
                    _context.Update(current_outpatientClinic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!outpatient_clinicExists(edit_outpatientClinic.outpatient_clinic_ID))
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
            //ViewData["outpatient_clinic_timeslot_ID"] = new SelectList(_context.outpatient_clinic_timeslots, "outpatient_clinic_timeslot_ID", "outpatient_clinic_timeslot_name", outpatient_clinic_ViewModel.outpatient_clinic_timeslot_ID);
            //ViewData["vet_ID"] = new SelectList(_context.vet_informations, "vet_ID", "vet_ID", outpatient_clinic_ViewModel.vet_ID);
            return NotFound();
        }

        // GET: outpatient_clinic/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var outpatient_clinic = await _context.outpatient_clinics
        //        .Include(o => o.outpatient_clinic_timeslot)
        //        .Include(o => o.vet)
        //        .FirstOrDefaultAsync(m => m.outpatient_clinic_ID == id);
        //    if (outpatient_clinic == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(outpatient_clinic);
        //}

        // POST: outpatient_clinic/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var outpatient_clinic = await _context.outpatient_clinics.FindAsync(id);
        //    if (outpatient_clinic != null)
        //    {
        //        _context.outpatient_clinics.Remove(outpatient_clinic);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool outpatient_clinicExists(int id)
        {
            return _context.outpatient_clinics.Any(e => e.outpatient_clinic_ID == id);
        }
    }
}
