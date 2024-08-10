﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tailstale.MedRecordDTO;
using Tailstale.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Tailstale.Controllers
{
    public class nursing_recordController : Controller
    {
        private readonly TailstaleContext _context;

        public nursing_recordController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: nursing_record
        public async Task<IActionResult> Index(int hosp_record_id)
        {
            //var records = await (from n in _context.nursing_records
            //                     join p in _context.pets on n.pet_id equals p.pet_ID
            //                     join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
            //                     from v in vsrGroup.DefaultIfEmpty()
            //                     join h in _context.hosp_records on n.hosp_record_id equals h.id
            //                     where n.hosp_record_id == hosp_record_id //鎖定h_h_id
            //                     select new NursingDTO
            //                     {
            //                         id = n.id,
            //                         pet_id = p.pet_ID,
            //                         hosp_record_id = h.id,
            //                         datetime = n.datetime,
            //                         weight = n.weight,
            //                         memo = n.memo,
            //                         vs_id = v.id,
            //                     })
            //                    .ToListAsync();
            //var pet_id = await (from h in _context.hosp_records
            //                    join m in _context.medical_records on h.medical_record_id equals m.id
            //                    where h.id == hosp_record_id
            //                    select m.pet_id)
            //       .FirstOrDefaultAsync();

            //ViewBag.hosp_record_id = hosp_record_id;
            //ViewBag.pet_id = pet_id;
            //var sortedRecord = records.OrderByDescending(n => n.datetime);
            return View(/*sortedRecord*/);
        }

        // GET: nursing_record/Details/5 ?GPT版
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            ////var records = from n in _context.nursing_records
            //              //join p in _context.pets on n.pet_id equals p.pet_ID
            //              //join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
            //              from v in vsrGroup.DefaultIfEmpty()
            //              //join h in _context.hosp_records on n.hosp_record_id equals h.id
            //              orderby n.datetime descending
            //              select new NursingDTO
            //              {
            //                  id = n.id,
            //                  pet_id = p.pet_ID,
            //                  hosp_record_id = h.id,
            //                  datetime = n.datetime,
            //                  weight = n.weight,
            //                  memo = n.memo,
            //                  vs_id = v.id
            //              };

            //if (records == null || !records.Any())
            //{
            //    return NotFound();
            //}

            return View();
        }


        // GET: nursing_record/Create
        public IActionResult Create(int hosp_record_id)
        {
            //var pet_id = (from h in _context.hosp_records
            //              join m in _context.medical_records on h.medical_record_id equals m.id
            //              where h.id == hosp_record_id
            //              select m.pet_id).FirstOrDefault();
            //var model = new NursingDTO
            //{
            //    hosp_record_id = hosp_record_id,
            //    pet_id = pet_id,
            //    datetime = DateTime.Now,
            //};

            //ViewBag.hosp_record_id = hosp_record_id;
            //ViewBag.pet_id = pet_id;
            return View(/*model*/);
        }

        // POST: nursing_record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] NursingDTO nursingDTO)
        {
            var a = new nursing_record
            {
                id = nursingDTO.id,
                //pet_id = nursingDTO.pet_id,
                //hosp_record_id = nursingDTO.hosp_record_id,
                datetime = nursingDTO.datetime,
                weight = nursingDTO.weight,
                memo = nursingDTO.memo,
                //vital_sign_record_id = nursingDTO.vs_id
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            //return Redirect(/*$"https://localhost:7112/nursing_record?hosp_record_id={a.hosp_record_id}"*/);

            return View();
        }

        // GET: nursing_record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var record = (from n in _context.nursing_records
            //              join p in _context.pets on n.pet_id equals p.pet_ID
            //              join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
            //              from v in vsrGroup.DefaultIfEmpty()
            //              join h in _context.hosp_records on n.hosp_record_id equals h.id
            //              where n.id == id && p.pet_ID == id
            //              select new NursingDTO
            //              {
            //                  id = n.id,
            //                  pet_id = p.pet_ID,
            //                  hosp_record_id = h.id,
            //                  datetime = n.datetime,
            //                  weight = n.weight,
            //                  memo = n.memo,
            //                  vs_id = v.id
            //              }).FirstOrDefault();
            //if (record == null)
            //{
            //    return NotFound();
            //}

            return View(/*record*/);
        }

        // POST: nursing_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] NursingDTO nursingDTO)
        {
            if (id != null)
            {
                return NotFound();
            }

            var record = new nursing_record
            {
                id = nursingDTO.id,
                //pet_id = nursingDTO.pet_id,
                //hosp_record_id = nursingDTO.id,
                datetime = nursingDTO.datetime,
                weight = nursingDTO.weight,
                memo = nursingDTO.memo,
                //vital_sign_record_id = nursingDTO.vs_id
            };
            _context.Update(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        ////GET: nursing_record/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var records = from n in _context.nursing_records
        //                  join p in _context.pets on n.pet_id equals p.pet_ID
        //                  join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id
        //                  join h in _context.hosp_records on n.hosp_record_id equals h.id
        //                  orderby n.datetime descending
        //                  where n.id == id
        //                  select new NursingDTO
        //                  {
        //                      id = n.id,
        //                      pet_id = p.pet_ID,
        //                      hosp_record_id = h.id,
        //                      datetime = n.datetime,
        //                      weight = n.weight,
        //                      memo = n.memo,
        //                      vs_id = v.id,
        //                  };
        //    if (records == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(records);
        //}

        // POST: nursing_record/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var nursing_record = await _context.nursing_records.FindAsync(id);
            if (nursing_record == null)
            {
                return Json(new { success = false, message = "找不到記錄" });
            }
            try
            {
                _context.nursing_records.Remove(nursing_record);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "刪除成功" });
            }
            catch (Exception ex)
            {
                // 記錄異常
                return Json(new { success = false, message = "刪除失敗" });
            }
        }

        private bool nursing_recordExists(int id)
        {
            return _context.nursing_records.Any(e => e.id == id);
        }
    }
}
