using Microsoft.AspNetCore.Mvc;
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

        // GET: medical_record
        public async Task<IActionResult> Index(int? pet_id)
        { //渲染
            var query = from m in _context.medical_records
                        join o in _context.outpatient_clinics on m.outpatient_clinic_id equals o.outpatient_clinic_ID
                        join p in _context.pets on m.pet_id equals p.pet_ID
                        join pet_type in _context.pet_types on p.pet_type_ID equals pet_type.ID
                        join k in _context.keepers on p.keeper_ID equals k.ID
                        select new MedicalRecordDTO
                        {  //DTO設的名字 = table抓出來的名字
                            id = m.id,
                            keeper_id = k.ID,
                            keeper_number = k.phone,
                            keeper_name = k.name,
                            pet_id = p.pet_ID,
                            pet_name = p.name,
                            species = pet_type.species,
                            pet_breed = pet_type.breed,
                            pet_age = p.age,
                            Datetime = m.Datetime,
                            outpatient_clinic_id = o.outpatient_clinic_ID,
                            weight = m.weight,
                            memo = m.memo,
                        };

            if (pet_id.HasValue)
            {
                query = query.Where(r => r.pet_id == pet_id.Value);

                var basicInfo = await _context.pets
                        .Where(p => p.pet_ID == pet_id)
                        .Select(p => new
                        {
                            keeper_name = p.keeper.name,
                            pet_name = p.name,
                            species = p.pet_type.species,
                            pet_breed = p.pet_type.breed,
                            pet_age = p.age
                        }).FirstOrDefaultAsync();
                ViewBag.basicInfo = basicInfo;
            }
            var records = await query.OrderByDescending(m => m.Datetime).ToListAsync();
            ViewBag.pet_id = pet_id;

            return View(records);
        }

        // GET: medical_record/Create
        [HttpGet]
        public IActionResult Create(int pet_id)
        {
            ViewBag.pet_id = pet_id;
            return View();
        }

        // POST: medical_record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int pet_id, [FromForm] MedicalRecordDTO medicalRecordDTO)
        {
            var a = new medical_record
            {
                pet_id = medicalRecordDTO.pet_id,
                Datetime = medicalRecordDTO.Datetime,
                weight = medicalRecordDTO.weight,
                outpatient_clinic_id = medicalRecordDTO.outpatient_clinic_id,
                complain = medicalRecordDTO.complain,
                diagnosis = medicalRecordDTO.diagnosis,
                memo = medicalRecordDTO.memo,
                fee = medicalRecordDTO.fee
            };
            _context.Add(a);
            await _context.SaveChangesAsync();
            return Redirect($"https://localhost:7112/medical_record?pet_id={a.pet_id}");
        }


        // GET: medical_record/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var medical_record = (from r in _context.medical_records
                                  join o in _context.outpatient_clinics on r.outpatient_clinic_id equals o.outpatient_clinic_ID
                                  join p in _context.pets on r.pet_id equals p.pet_ID
                                  join k in _context.keepers on p.keeper_ID equals k.ID
                                  where r.id == id
                                  select new MedicalRecordDTO
                                  {
                                      id = r.id,
                                      keeper_id = k.ID,
                                      keeper_name = k.name,
                                      pet_id = p.pet_ID,
                                      pet_name = p.name,
                                      Datetime = r.Datetime,
                                      outpatient_clinic_id = o.outpatient_clinic_ID,
                                      weight = r.weight,
                                      complain = r.complain,
                                      diagnosis = r.diagnosis,
                                      memo = r.memo,
                                      fee = r.fee,
                                  }).FirstOrDefault();
            if (medical_record == null)
            {
                return NotFound();
            }
            ViewBag.medical_records_id = id;

            var pet_id = medical_record.pet_id;
            ViewBag.pet_id = pet_id;

            return View(medical_record);
        }

        // GET: medical_record/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = (from m in _context.medical_records
                          join o in _context.outpatient_clinics on m.outpatient_clinic_id equals o.outpatient_clinic_ID
                          join p in _context.pets on m.pet_id equals p.pet_ID
                          join k in _context.keepers on p.keeper_ID equals k.ID
                          where m.id == id
                          select new MedicalRecordDTO
                          {
                              id = m.id,
                              keeper_id = k.ID,
                              pet_id = p.pet_ID,
                              Datetime = m.Datetime,
                              outpatient_clinic_id = o.outpatient_clinic_ID,
                              weight = m.weight,
                              complain = m.complain,
                              diagnosis = m.diagnosis,
                              memo = m.memo,
                              fee = m.fee
                          }).FirstOrDefault(); //FirstOrDefault嗽嘎嘍啊
            if (record == null)
            {
                return NotFound();
            }

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
                outpatient_clinic_id = medicalRecordDTO.outpatient_clinic_id,
                Datetime = medicalRecordDTO.Datetime,
                weight = medicalRecordDTO.weight,
                complain = medicalRecordDTO.complain,
                diagnosis = medicalRecordDTO.diagnosis,
                memo = medicalRecordDTO.memo,
                fee = medicalRecordDTO.fee
            };
            _context.Update(a);
            await _context.SaveChangesAsync();

            return Redirect($"https://localhost:7112/medical_record?pet_id={a.pet_id}");
        }
    }
}