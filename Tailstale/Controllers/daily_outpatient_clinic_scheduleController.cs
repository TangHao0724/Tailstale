using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hospital_DTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class daily_outpatient_clinic_scheduleController : Controller
    {
        private readonly TailstaleContext _context;

        public daily_outpatient_clinic_scheduleController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: daily_outpatient_clinic_schedule
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.daily_outpatient_clinic_schedules.Include(d => d.outpatient_clinic);

            

            return View(await tailstaleContext.ToListAsync());
        }

        //GET:daily_outpatient_clinic_schedule/GetSchedule/2024-08-01
        [HttpGet]
        public async Task<IEnumerable<daily_outpatient_clinic_schedule_DTO>> GetSchedule(DateOnly id)
        {
            var targetYear = id.Year;
            var targetMonth = id.Month;

            var docs_DTO = await(from docs in _context.daily_outpatient_clinic_schedules
                           join opc in _context.outpatient_clinics
                           on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                           join vet_Info in _context.vet_informations
                           on opc.vet_ID equals vet_Info.vet_ID
                           join opct in _context.outpatient_clinic_timeslots
                           on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                           where docs.date.HasValue && docs.date.Value.Year == targetYear && docs.date.Value.Month == targetMonth
                           orderby opct.startat
                           select new daily_outpatient_clinic_schedule_DTO
                           {
                               daily_outpatient_clinic_schedule_ID=docs.daily_outpatient_clinic_schedule_ID,
                               date = docs.date,
                               outpatient_clinic_name = opc.outpatient_clinic_name,
                               outpatient_clinic_ID = opc.outpatient_clinic_ID,
                               vet_name = vet_Info.vet_name,
                               outpatient_clinic_timeslot_name = opct.outpatient_clinic_timeslot_name,
                               startat = opct.startat,
                               endat = opct.endat,
                               daily_outpatient_clinic_schedule_status = docs.daily_outpatient_clinic_schedule_status
                           }).ToListAsync();
            return docs_DTO;
        }

        // GET: daily_outpatient_clinic_schedule/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var daily_outpatient_clinic_schedule = await _context.daily_outpatient_clinic_schedules
        //        .Include(d => d.outpatient_clinic)
        //        .FirstOrDefaultAsync(m => m.daily_outpatient_clinic_schedule_ID == id);
        //    if (daily_outpatient_clinic_schedule == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(daily_outpatient_clinic_schedule);
        //}

        //GET:daily_outpatient_clinic_schedule/CreateOptions/id
        public async Task<IEnumerable> CreateOptions(string id)
        {
            var options = await(from opc in _context.outpatient_clinics
                          join vetInfo in _context.vet_informations
                          on opc.vet_ID equals vetInfo.vet_ID
                          join opct in _context.outpatient_clinic_timeslots
                          on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                          where vetInfo.business_ID == 1003
                          && opc.dayofweek == $"{id}"
                          && opc.status==false
                          orderby opct.startat
                          select new
                          {
                              outpatient_clinic_ID = opc.outpatient_clinic_ID,
                              outpatient_clinic_Info = $"{opc.outpatient_clinic_name},{vetInfo.vet_name},{opct.outpatient_clinic_timeslot_name}"
                          }).ToListAsync();
            //var outpatient_clinic_options = new SelectList(options, "outpatient_clinic_ID", "outpatient_clinic_Info");

            return options;
        }


        // GET: daily_outpatient_clinic_schedule/Create
        //public IActionResult Create()
        //{
        //    ViewData["outpatient_clinic_ID"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "outpatient_clinic_name");
        //    return View();
        //}

        // POST: daily_outpatient_clinic_schedule/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<bool> Create([FromBody]create_daily_outpatient_clinic_schedule_DTO daily_outpatient_clinic_schedule)
        {
            bool docsExists = _context.daily_outpatient_clinic_schedules.Any(d => d.outpatient_clinic_ID == daily_outpatient_clinic_schedule.outpatient_clinic_ID && d.date == daily_outpatient_clinic_schedule.date);
            if (docsExists)
            {
                return false;
            }
            
            daily_outpatient_clinic_schedule docs = new daily_outpatient_clinic_schedule()
            {
                date = daily_outpatient_clinic_schedule.date,
                outpatient_clinic_ID = daily_outpatient_clinic_schedule.outpatient_clinic_ID,
                daily_outpatient_clinic_schedule_status = true,
            };
            try
            {
                _context.daily_outpatient_clinic_schedules.Add(docs);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return false;
            }
            //_context.daily_outpatient_clinic_schedules.Add(docs);
            //await _context.SaveChangesAsync();
            return true;
            
        }

        // GET: daily_outpatient_clinic_schedule/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var daily_outpatient_clinic_schedule = await _context.daily_outpatient_clinic_schedules.FindAsync(id);
        //    if (daily_outpatient_clinic_schedule == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["outpatient_clinic_ID"] = new SelectList(_context.outpatient_clinics, "outpatient_clinic_ID", "outpatient_clinic_name", daily_outpatient_clinic_schedule.outpatient_clinic_ID);
        //    return View(daily_outpatient_clinic_schedule);
        //}

        // PUT: daily_outpatient_clinic_schedule/Edit/5        
        [HttpPut]
        public async Task<bool> Edit(int id)
        {
            var currentdocs = await _context.daily_outpatient_clinic_schedules.FindAsync(id);

            if (currentdocs == null)
            {
                return false;
            }            
            try
            {
                if (currentdocs.daily_outpatient_clinic_schedule_status == true)
                {
                    currentdocs.daily_outpatient_clinic_schedule_status =false;
                }
                else
                {
                    currentdocs.daily_outpatient_clinic_schedule_status = true;
                }
                _context.Update(currentdocs);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        // GET: daily_outpatient_clinic_schedule/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var daily_outpatient_clinic_schedule = await _context.daily_outpatient_clinic_schedules
        //        .Include(d => d.outpatient_clinic)
        //        .FirstOrDefaultAsync(m => m.daily_outpatient_clinic_schedule_ID == id);
        //    if (daily_outpatient_clinic_schedule == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(daily_outpatient_clinic_schedule);
        //}

        // Delete: daily_outpatient_clinic_schedule/DeleteConfirmed
        [HttpDelete]
        public async Task<bool> DeleteConfirmed(int id)
        {
            var daily_outpatient_clinic_schedule = await _context.daily_outpatient_clinic_schedules.FindAsync(id);
            if (daily_outpatient_clinic_schedule == null)
            {
                return false;
            }
            try
            {
                _context.daily_outpatient_clinic_schedules.Remove(daily_outpatient_clinic_schedule);
                await _context.SaveChangesAsync();
            }
            catch
            { 
                return false;
            }            
            return true;
        }

        private bool daily_outpatient_clinic_scheduleExists(int id)
        {
            return _context.daily_outpatient_clinic_schedules.Any(e => e.daily_outpatient_clinic_schedule_ID == id);
        }
    }
}
