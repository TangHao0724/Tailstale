using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.MedRecordDTO;
using Tailstale.Models;

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
        public async Task<IActionResult> Index()
        {
            var records = from n in _context.nursing_records
                          join p in _context.pets on n.pet_id equals p.pet_ID
                          join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id
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
                              VS_id = v.id,
                          };
            return View(records);
        }

        //原先的Details GET
        // GET: nursing_record/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var nursing_record = await _context.nursing_records
        //        .Include(n => n.pet)
        //        .Include(n => n.vital_sign_record)
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (nursing_record == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(nursing_record);
        //}

        // GET: nursing_record/Details/5 ?GPT版
        public async Task<IActionResult> Details(int? hosp_history_id)
        {
            if (hosp_history_id == null)
            {
                return NotFound();
            }

            var records = from n in _context.nursing_records
                          join p in _context.pets on n.pet_id equals p.pet_ID
                          join v in _context.vital_sign_records on n.vital_sign_record_id equals v.id
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
                              VS_id = v.id,
                          };

            if (records == null || !records.Any())
            {
                return NotFound();
            }

            return View(records);
        }
 

    // GET: nursing_record/Create
    public IActionResult Create()
        {
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID");
            ViewData["vital_sign_record_id"] = new SelectList(_context.vital_sign_records, "id", "id");
            return View();
        }

        // POST: nursing_record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,pet_id,datetime,weight,memo,vital_sign_record_id,biological_test_id")] nursing_record nursing_record)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nursing_record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", nursing_record.pet_id);
            ViewData["vital_sign_record_id"] = new SelectList(_context.vital_sign_records, "id", "id", nursing_record.vital_sign_record_id);
            return View(nursing_record);
        }

        // GET: nursing_record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nursing_record = await _context.nursing_records.FindAsync(id);
            if (nursing_record == null)
            {
                return NotFound();
            }
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", nursing_record.pet_id);
            ViewData["vital_sign_record_id"] = new SelectList(_context.vital_sign_records, "id", "id", nursing_record.vital_sign_record_id);
            return View(nursing_record);
        }

        // POST: nursing_record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,pet_id,datetime,weight,memo,vital_sign_record_id,biological_test_id")] nursing_record nursing_record)
        {
            if (id != nursing_record.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nursing_record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!nursing_recordExists(nursing_record.id))
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
            ViewData["biological_test_id"] = new SelectList(_context.biological_tests, "id", "id");
            ViewData["pet_id"] = new SelectList(_context.pets, "pet_ID", "pet_ID", nursing_record.pet_id);
            ViewData["vital_sign_record_id"] = new SelectList(_context.vital_sign_records, "id", "id", nursing_record.vital_sign_record_id);
            return View(nursing_record);
        }

        // GET: nursing_record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nursing_record = await _context.nursing_records
                .Include(n => n.pet)
                .Include(n => n.vital_sign_record)
                .FirstOrDefaultAsync(m => m.id == id);
            if (nursing_record == null)
            {
                return NotFound();
            }

            return View(nursing_record);
        }

        // POST: nursing_record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nursing_record = await _context.nursing_records.FindAsync(id);
            if (nursing_record != null)
            {
                _context.nursing_records.Remove(nursing_record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool nursing_recordExists(int id)
        {
            return _context.nursing_records.Any(e => e.id == id);
        }
    }
}
