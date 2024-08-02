﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.MedRecordDTO;
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

        public async Task<IActionResult> appointment()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _context.Appointments
                .Where(a => a.daily_outpatient_clinic_schedule.date == today)
                .Select(a => new
                {
                    keeper_ID = a.keeper_ID,
                    keeper_name = a.keeper.name,
                    pet_ID = a.pet_ID,
                    pet_name = a.pet.name,
                    date = a.daily_outpatient_clinic_schedule.date
                })
                .ToListAsync();

            ViewBag.selected = appointments;
            return View();
        }

        // GET: medical_record
        public async Task<IActionResult> Index(int? pet_id)
        { //渲染
            var query = from m in _context.medical_records
                        join o in _context.outpatient_clinics on m.outpatient_clinic_id equals o.outpatient_clinic_ID
                        join p in _context.pets on m.pet_id equals p.pet_ID
                        join k in _context.keepers on p.keeper_ID equals k.ID
                        orderby m.created_at descending
                        select new MedicalRecordDTO
                        {  //DTO設的名字 = table抓出來的名字
                            id = m.id,
                            keeper_id = k.ID,
                            keeper_num = k.phone,
                            pet_id = p.pet_ID,
                            pet_name = p.name,
                            created_at = m.created_at,
                            outpatient_clinic_id = o.outpatient_clinic_ID,
                            weight = m.weight,
                            memo = m.memo,
                        };

            if (pet_id.HasValue)
            {
                query = query.Where(r => r.pet_id == pet_id.Value);
            }

            var records = await query.OrderByDescending(m => m.created_at).ToListAsync();

            return View(records);
        }

        // GET: medical_record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_record = (from r in _context.medical_records
                                  join o in _context.outpatient_clinics on r.outpatient_clinic_id equals o.outpatient_clinic_ID
                                  join p in _context.pets on r.pet_id equals p.pet_ID
                                  join k in _context.keepers on p.keeper_ID equals k.ID
                                  where r.id == id /*鎖定id*/
                                  select new MedicalRecordDTO
                                  {
                                      id = r.id,
                                      keeper_id = k.ID,
                                      keeper_name = k.name,
                                      pet_id = p.pet_ID,
                                      pet_name = p.name,
                                      created_at = r.created_at,
                                      outpatient_clinic_id = o.outpatient_clinic_ID,
                                      weight = r.weight,
                                      admission_process = r.admission_process,
                                      diagnosis = r.diagnosis,
                                      treatment = r.treatment,
                                      memo = r.memo,
                                      fee = r.fee,
                                  }).FirstOrDefault();

            if (medical_record == null)
            {
                return NotFound();
            }

            return View(medical_record);
        }

        // GET: medical_record/Create
        [HttpGet]
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
        public async Task<IActionResult> Create([FromForm] MedicalRecordDTO medicalRecordDTO)
        {
            var a = new medical_record
            {
                keeper_id = medicalRecordDTO.keeper_id,
                pet_id = medicalRecordDTO.pet_id,
                created_at = medicalRecordDTO.created_at,
                weight = medicalRecordDTO.weight,
                outpatient_clinic_id = medicalRecordDTO.outpatient_clinic_id,
                admission_process = medicalRecordDTO.admission_process,
                diagnosis = medicalRecordDTO.diagnosis,
                treatment = medicalRecordDTO.treatment,
                memo = medicalRecordDTO.memo,
                fee = medicalRecordDTO.fee
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: medical_record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = (from e in _context.medical_records
                          join o in _context.outpatient_clinics on e.outpatient_clinic_id equals o.outpatient_clinic_ID
                          join p in _context.pets on e.pet_id equals p.pet_ID
                          join k in _context.keepers on p.keeper_ID equals k.ID
                          where e.id == id
                          select new MedicalRecordDTO
                          {
                              id = e.id,
                              keeper_id = k.ID,
                              pet_id = p.pet_ID,
                              created_at = e.created_at,
                              outpatient_clinic_id = o.outpatient_clinic_ID,
                              weight = e.weight,
                              admission_process = e.admission_process,
                              diagnosis = e.diagnosis,
                              treatment = e.treatment,
                              memo = e.memo,
                              fee = e.fee
                          }).FirstOrDefault(); //FirstOrDefault嗽嘎嘍啊

            return View(record);
        }

        // POST: medical_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] MedicalRecordDTO medicalRecordDTO)
        {
            if (id != medicalRecordDTO.id)
            {
                return NotFound();
            }
            //if (ModelState.IsValid)
            //{
            //try
            //{
            var a = new medical_record
            {
                id = id,
                pet_id = medicalRecordDTO.pet_id,
                created_at = medicalRecordDTO.created_at,
                weight = medicalRecordDTO.weight,
                outpatient_clinic_id = medicalRecordDTO.outpatient_clinic_id,
                admission_process = medicalRecordDTO.admission_process,
                diagnosis = medicalRecordDTO.diagnosis,
                treatment = medicalRecordDTO.treatment,
                memo = medicalRecordDTO.memo,
                fee = medicalRecordDTO.fee
            };
            _context.Update(a);
            await _context.SaveChangesAsync();
            return Redirect($"https://localhost:7112/medical_record?pet_id={a.pet_id}");
            //}
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!medical_recordExists(medicalRecordDTO.id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    //return RedirectToAction(nameof(Index));
            //}
            //return View(medicalRecordDTO); //沒成功
        }

        // GET: medical_record/Delete/5   // medical_record不可以delete！！！
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var medical_record = (from r in _context.medical_records
        //                          join o in _context.outpatient_clinics on r.outpatient_clinic_id equals o.outpatient_clinic_ID
        //                          join p in _context.pets on r.pet_id equals p.pet_ID
        //                          join k in _context.keepers on p.keeper_ID equals k.ID
        //                          where r.id == id
        //                          select new MedicalRecordDTO
        //                          {
        //                              keeper_id = k.ID,
        //                              pet_id = p.pet_ID,
        //                              created_at = r.created_at,
        //                              outpatient_clinic_id = o.outpatient_clinic_ID,
        //                              weight = r.weight,
        //                              admission_process = r.admission_process,
        //                              diagnosis = r.diagnosis,
        //                              treatment = r.treatment,
        //                              memo = r.memo,
        //                          }).FirstOrDefault();

        //    if (medical_record == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(medical_record);
        //}

        //// POST: medical_record/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var medical_record = await _context.medical_records.FindAsync(id);
        //    if (medical_record != null)
        //    {
        //        _context.medical_records.Remove(medical_record);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool medical_recordExists(int id)
        //{
        //    return _context.medical_records.Any(e => e.id == id);
        //}
    }
}
