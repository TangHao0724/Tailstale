﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tailstale.MedRecordDTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class HospController : Controller
    {
        private readonly TailstaleContext _context;

        public HospController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Hosp
        [HttpGet]
        public async Task<IActionResult> Inpatient()
        {
            return View();
        }

            // GET: Hosp
            [HttpGet]
        public async Task<IActionResult> hosp_history(int pet_id)
        {
            var query = await (from p in _context.pets
                               join m in _context.medical_records on p.pet_ID equals m.pet_id
                               join h in _context.hosp_records on m.id equals h.medical_records_id
                               join w in _context.wards on h.ward_id equals w.ward_ID into wardGroup
                               from w in wardGroup.DefaultIfEmpty()
                               where p.pet_ID == pet_id
                               select new HospDTO
                               {
                                   pet_id = pet_id,
                                   id = h.id,
                                   medical_records_id = m.id,
                                   Datetime = m.Datetime,
                                   admission_date = h.admission_date,
                                   discharge_date = h.discharge_date,
                                   ward_id = w != null ? w.ward_ID : null,
                               })
                                .GroupBy(h => h.id)
                                .Select(g => g.First())
                                .ToListAsync();

            var sorted = query.OrderByDescending(h => h.admission_date);

            var basicInfo = await _context.pets
                        .Where(p => p.pet_ID == pet_id)
                        .Select(p => new
                        {
                            keeper_name = p.keeper.name,
                            pet_name = p.name,
                            species = p.pet_type.species,
                            pet_breed = p.pet_type.breed,
                            pet_age = p.age,

                        }).FirstOrDefaultAsync();
            ViewBag.basicInfo = basicInfo;
            ViewBag.pet_id = pet_id;

            return View(sorted);
        }

        // GET: Hosp
        [HttpGet]
        public async Task<IActionResult> Index(int medical_records_id)
        {
            var Hosp = await (from h in _context.hosp_records
                              join m in _context.medical_records on h.medical_records_id equals m.id
                              join w in _context.wards on h.ward_id equals w.ward_ID into wardGroup
                              from w in wardGroup.DefaultIfEmpty()
                              where h.medical_records_id == medical_records_id
                              select new HospDTO
                              {
                                  id = h.id,
                                  medical_records_id = medical_records_id,
                                  admission_date = h.admission_date,
                                  discharge_date = h.discharge_date,
                                  Datetime = m.Datetime,
                                  ward_id = w != null ? w.ward_ID : null,
                                  memo = h.memo,
                                  pet_id = m.pet_id
                              })
                        .GroupBy(h => h.id)
                        .Select(g => g.First())
                        .ToListAsync();

            var sortedHosp = Hosp.OrderByDescending(h => h.admission_date);
            if (Hosp == null)
            {
                return NotFound();
            }
            var basicInfo = await _context.medical_records.Where(b => b.id == medical_records_id)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.keeper)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.pet_type)
                    .Select(b => new
                    {
                        keeper_name = b.pet.keeper.name,
                        pet_name = b.pet.name,
                        species = b.pet.pet_type.species,
                        pet_breed = b.pet.pet_type.breed,
                        pet_age = b.pet.age
                    }).FirstOrDefaultAsync();
            ViewBag.basicInfo = basicInfo;
            ViewBag.medical_records_id = medical_records_id;

            var pet_id = await _context.medical_records.Where(m => m.id == medical_records_id)
                                                .Include(m => m.pet)
                                                .Select(m => m.pet.pet_ID).FirstOrDefaultAsync();
            ViewBag.pet_id = pet_id;

            return View(sortedHosp);
        }

        // GET: Hosp/Create
        [HttpGet]
        public IActionResult Create(int medical_records_id)
        { //初始化器
            var model = new HospDTO
            {
                medical_records_id = medical_records_id,
                admission_date = DateTime.Now
            };

            var basicInfo = _context.medical_records.Where(b => b.id == medical_records_id)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.keeper)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.pet_type)
                    .Select(b => new
                    {
                        keeper_name = b.pet.keeper.name,
                        pet_name = b.pet.name,
                        species = b.pet.pet_type.species,
                        pet_breed = b.pet.pet_type.breed,
                        pet_age = b.pet.age
                    }).FirstOrDefault();
            ViewBag.basicInfo = basicInfo;

            return View(model);
        }

        // POST: Hosp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] HospDTO hospDTO)
        {
            var a = new hosp_record
            {
                medical_records_id = hospDTO.medical_records_id,
                admission_date = hospDTO.admission_date,
                discharge_date = hospDTO.discharge_date,
                ward_id = hospDTO.ward_id,
                memo = hospDTO.memo
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            return Redirect($"https://localhost:7112/Hosp?medical_records_id={a.medical_records_id}");
        }

        // GET: Hosp/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hosp = await (from h in _context.hosp_records
                              join m in _context.medical_records on h.medical_records_id equals m.id
                              join w in _context.wards on h.ward_id equals w.ward_ID
                              where h.id == id
                              select new HospDTO
                              {
                                  id = h.id,
                                  medical_records_id = m.id,
                                  admission_date = h.admission_date,
                                  discharge_date = h.discharge_date,
                                  ward_id = w.ward_ID,
                                  memo = h.memo
                              }).FirstOrDefaultAsync();

            var medical_records_id = _context.hosp_records.Where(h => h.id == id)
                                                          .Select(h => h.medical_records.id)
                                                          .FirstOrDefault();

            var basicInfo = _context.medical_records.Where(b => b.id == medical_records_id)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.keeper)
                    .Include(b => b.pet)
                        .ThenInclude(p => p.pet_type)
                    .Select(b => new
                    {
                        keeper_name = b.pet.keeper.name,
                        pet_name = b.pet.name,
                        pet_id = b.pet.pet_ID,
                        species = b.pet.pet_type.species,
                        pet_breed = b.pet.pet_type.breed,
                        pet_age = b.pet.age
                    }).FirstOrDefault();
            ViewBag.basicInfo = basicInfo;
            ViewBag.pet_id = basicInfo.pet_id;

            return View(hosp);
        }

        // POST: Hosp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] HospDTO hospDTO)
        {
            if (id != hospDTO.id)
            {
                return NotFound();
            }
            var h = new hosp_record
            {
                id = hospDTO.id,
                medical_records_id = hospDTO.medical_records_id,
                admission_date = hospDTO.admission_date,
                discharge_date = hospDTO.discharge_date,
                ward_id = hospDTO.ward_id,
                memo = hospDTO.memo
            };
            _context.Update(h);
            await _context.SaveChangesAsync();

            return Redirect($"https://localhost:7112/Hosp?medical_records_id={h.medical_records_id}");
        }
    }
}
