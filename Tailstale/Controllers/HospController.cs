using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index(int medical_record_id)
        {
            var Hosp = await (from h in _context.hosp_histories
                              join m in _context.medical_records on h.medical_record_id equals m.id
                              join w in _context.wards on h.ward_id equals w.ward_ID into wardGroup
                              from w in wardGroup.DefaultIfEmpty()
                              join n in _context.nursing_records on h.id equals n.hosp_history_id into nursingGroup
                              from n in nursingGroup.DefaultIfEmpty()
                              where h.medical_record_id == medical_record_id
                              select new HospDTO
                              {
                                  id = h.id,
                                  medical_record_id = medical_record_id,
                                  admission_date = h.admission_date,
                                  discharge_date = h.discharge_date,
                                  created_at = m.created_at,
                                  nursing_record_id = n != null ? n.id : (int?)null,
                                  nursing_record_datetime = n != null ? n.datetime : (DateTime?)null,
                                  ward_id = w != null ? w.ward_ID : null,
                                  memo = h.memo,
                                  pet_id = m.pet_id
                              })
                        .GroupBy(h => h.id)
                        .Select(g => g.First())
                        .ToListAsync();

            var sortedHosp = Hosp.OrderByDescending(h => h.admission_date);
            ViewBag.medical_record_id = medical_record_id; //用來傳medical_record_id給新增的記錄
            ViewBag.pet_id = Hosp.FirstOrDefault()?.pet_id;

            return View(sortedHosp);
        }

        // GET: Hosp/Create
        [HttpGet]
        public IActionResult Create(int medical_record_id)
        { //初始化器
            var model = new HospDTO
            {
                medical_record_id = medical_record_id,
                admission_date = DateTime.Now,
            };
            ViewBag.medical_record_id = medical_record_id;
            return View(model);
        }

        // POST: Hosp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] HospDTO hospDTO)
        {
            var a = new hosp_history
            {
                medical_record_id = hospDTO.medical_record_id,
                admission_date = hospDTO.admission_date,
                discharge_date = hospDTO.discharge_date,
                ward_id = hospDTO.ward_id,
                memo = hospDTO.memo
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            return Redirect($"https://localhost:7112/Hosp?medical_record_id={a.medical_record_id}");
        }

        // GET: Hosp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hosp = (from h in _context.hosp_histories
                        join m in _context.medical_records on h.medical_record_id equals m.id
                        join w in _context.wards on h.ward_id equals w.ward_ID
                        where h.id == id
                        select new HospDTO
                        {
                            id = h.id,
                            medical_record_id = m.id,
                            admission_date = h.admission_date,
                            discharge_date = h.discharge_date,
                            ward_id = w.ward_ID,
                            memo = h.memo
                        }).FirstOrDefault();
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
            var h = new hosp_history
            {
                id = hospDTO.id,
                medical_record_id = hospDTO.medical_record_id,
                admission_date = hospDTO.admission_date,
                discharge_date = hospDTO.discharge_date,
                ward_id = hospDTO.ward_id,
                memo = hospDTO.memo
            };
            _context.Update(h);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Hosp/Delete/5   // hosp_histories不可以delete！！！
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hosp_history = await _context.hosp_histories
        //        .Include(h => h.medical_record)
        //        .Include(h => h.ward)
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (hosp_history == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hosp_history);
        //}

        //// POST: Hosp/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var hosp_history = await _context.hosp_histories.FindAsync(id);
        //    if (hosp_history != null)
        //    {
        //        _context.hosp_histories.Remove(hosp_history);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool hosp_historyExists(int id)
        //{
        //    return _context.hosp_histories.Any(e => e.id == id);
        //}
    }
}
