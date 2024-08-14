using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tailstale.MedRecordDTO;
using Tailstale.Models;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> Info()
        {
            var records = from n in _context.nursing_records
                          join p in _context.pets on n.pet_id equals p.pet_ID
                          join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
                          from v in vsrGroup.DefaultIfEmpty()
                          join h in _context.hosp_histories on n.hosp_history_id equals h.id
                          orderby n.datetime descending
                          select new NursingDTO
                          {
                              id = n.id,
                              pet_id = p.pet_ID,
                              hosp_history_id = h.id,
                              datetime = n.datetime,
                              weight = n.weight,
                              memo = n.memo,
                              vs_id = v.id,
                          };
            return View(records);
        }

        // GET: nursing_record/Details/5 ?GPT版
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var records = from n in _context.nursing_records
            //              join p in _context.pets on n.pet_id equals p.pet_ID
            //              join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
            //              from v in vsrGroup.DefaultIfEmpty()
            //              join h in _context.hosp_histories on n.hosp_history_id equals h.id
            //              orderby n.datetime descending
            //              select new NursingDTO
            //              {
            //                  id = n.id,
            //                  pet_id = p.pet_ID,
            //                  hosp_history_id = h.id,
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
        public IActionResult Create()
        {
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["outpatient_clinic_id"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "name");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID");
            return View();
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
                pet_id = nursingDTO.pet_id,
               // hosp_histories = nursingDTO.hosp_history_id,
                datetime = nursingDTO.datetime,
                weight = nursingDTO.weight,
                memo = nursingDTO.memo,
                vital_sign_record_id = nursingDTO.vs_id
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            return RedirectToAction("Info");
        }

        // GET: nursing_record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = (from n in _context.nursing_records
                          join p in _context.pets on n.pet_id equals p.pet_ID
                          join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id into vsrGroup
                          from v in vsrGroup.DefaultIfEmpty()
                          join h in _context.hosp_histories on n.hosp_history_id equals h.id
                          where n.id == id
                          select new NursingDTO
                          {
                              id = n.id,
                              pet_id = p.pet_ID,
                              hosp_history_id = h.id,
                              datetime = n.datetime,
                              weight = n.weight,
                              memo = n.memo,
                              vs_id = v.id
                          }).FirstOrDefault();
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // POST: nursing_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] NursingDTO nursingDTO)
        {
            if (id != nursingDTO.id)
            {
                return NotFound();
            }

            //var record = new nursing_record
            //{
            //    id = nursingDTO.id,
            //    pet_id = nursingDTO.pet_id,
            //    hosp_history_id = nursingDTO.id,
            //    datetime = nursingDTO.datetime,
            //    weight = nursingDTO.weight,
            //    memo = nursingDTO.memo,
            //    vital_sign_record_id = nursingDTO.vs_id
            //};
            _context.Update(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Info");
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
        //                  join h in _context.hosp_histories on n.hosp_history_id equals h.id
        //                  orderby n.datetime descending
        //                  where n.id == id
        //                  select new NursingDTO
        //                  {
        //                      id = n.id,
        //                      pet_id = p.pet_ID,
        //                      hosp_history_id = h.id,
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
