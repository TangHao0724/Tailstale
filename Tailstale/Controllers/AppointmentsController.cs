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
    public class AppointmentsController : Controller
    {
        private readonly TailstaleContext _context;

        public AppointmentsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()//只顯示今天以後(包含今天)的查詢結果
        {
            var today = DateOnly.FromDateTime(DateTime.Today);//當天日期
            var Appointments = await(from A in _context.Appointments
                                   join docs in _context.daily_outpatient_clinic_schedules on A.daily_outpatient_clinic_schedule_ID equals docs.daily_outpatient_clinic_schedule_ID
                                   join opc in _context.outpatient_clinics on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                   join opct in _context.outpatient_clinic_timeslots on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                   join vInfo in _context.vet_informations on opc.vet_ID equals vInfo.vet_ID
                                   join pet in _context.pets on A.pet_ID equals pet.pet_ID
                                   join keeper in _context.keepers on A.keeper_ID equals keeper.ID
                                   join order_status in _context.order_statuses on A.Appointment_status equals order_status.ID
                                   where docs.date >= today
                                   select new Appointments_ViewModel
                                   {
                                       AppointmentID=A.Appointment_ID,
                                       Date = (DateOnly)docs.date,
                                       OutpatientClinicName = opc.outpatient_clinic_name,
                                       OutpatientClinicTimeslotName = opct.outpatient_clinic_timeslot_name,
                                       VetName = vInfo.vet_name,
                                       KeeperName = keeper.name,
                                       PetName = pet.name,
                                       PetID = pet.pet_ID, //連到病歷用
                                       AppointmentStatus = order_status.status_name
                                   }).ToListAsync();

            var statusOptions = new List<string> { "預約成功", "完成診療", "院方取消" , "預約未到" };
            ViewBag.StatusOptions = statusOptions;

            return View(Appointments);
        }

        //PUT:Appointments/AppointmentStatusChange/id
        [HttpPut]
        public async Task<bool> AppointmentStatusChange(int id, int statusId)//預約狀態修改
        {                       
            var currentAppointment= await _context.Appointments.FindAsync(id);
            if (currentAppointment == null)
            {
                return false;
            }
            try
            {
                currentAppointment.Appointment_status = statusId;

                _context.Update(currentAppointment);
                await _context.SaveChangesAsync();
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public async Task<IActionResult> Details()//只顯示今天以前的查詢結果
        {
            var today = DateOnly.FromDateTime(DateTime.Today);//當天日期
            var tailstaleContext = await (from A in _context.Appointments
                                          join docs in _context.daily_outpatient_clinic_schedules on A.daily_outpatient_clinic_schedule_ID equals docs.daily_outpatient_clinic_schedule_ID
                                          join opc in _context.outpatient_clinics on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                          join opct in _context.outpatient_clinic_timeslots on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                          join vInfo in _context.vet_informations on opc.vet_ID equals vInfo.vet_ID
                                          join pet in _context.pets on A.pet_ID equals pet.pet_ID
                                          join keeper in _context.keepers on A.keeper_ID equals keeper.ID
                                          join order_status in _context.order_statuses on A.Appointment_status equals order_status.ID
                                          where docs.date < today
                                          orderby docs.date descending
                                          select new Appointments_ViewModel
                                          {
                                              AppointmentID = A.Appointment_ID,
                                              Date = (DateOnly)docs.date,
                                              OutpatientClinicName = opc.outpatient_clinic_name,
                                              OutpatientClinicTimeslotName = opct.outpatient_clinic_timeslot_name,
                                              VetName = vInfo.vet_name,
                                              KeeperName = keeper.name,
                                              PetName = pet.name,
                                              AppointmentStatus = order_status.status_name
                                          }).ToListAsync();           

            return View(tailstaleContext);
        }

        // GET: Appointments/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointments
        //        .Include(a => a.Appointment_statusNavigation)
        //        .Include(a => a.daily_outpatient_clinic_schedule)
        //        .Include(a => a.keeper)
        //        .Include(a => a.pet)
        //        .FirstOrDefaultAsync(m => m.Appointment_ID == id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(appointment);
        //}

        //GET: Appointments/Create
        //public IActionResult Create()
        //{
        //    ViewData["Appointment_status"] = new SelectList(_context.order_statuses, "ID", "status_name");
        //    ViewData["daily_outpatient_clinic_schedule_ID"] = new SelectList(_context.daily_outpatient_clinic_schedules, "daily_outpatient_clinic_schedule_ID", "daily_outpatient_clinic_schedule_ID");
        //    ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email");
        //    ViewData["pet_ID"] = new SelectList(_context.pets, "pet_ID", "pet_ID");
        //    return View();
        //}

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Appointment_ID,pet_ID,keeper_ID,registration_time,daily_outpatient_clinic_schedule_ID,Appointment_status")] Appointment appointment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(appointment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Appointment_status"] = new SelectList(_context.order_statuses, "ID", "status_name", appointment.Appointment_status);
        //    ViewData["daily_outpatient_clinic_schedule_ID"] = new SelectList(_context.daily_outpatient_clinic_schedules, "daily_outpatient_clinic_schedule_ID", "daily_outpatient_clinic_schedule_ID", appointment.daily_outpatient_clinic_schedule_ID);
        //    ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", appointment.keeper_ID);
        //    ViewData["pet_ID"] = new SelectList(_context.pets, "pet_ID", "pet_ID", appointment.pet_ID);
        //    return View(appointment);
        //}

        // GET: Appointments/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointments.FindAsync(id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["Appointment_status"] = new SelectList(_context.order_statuses, "ID", "status_name", appointment.Appointment_status);
        //    ViewData["daily_outpatient_clinic_schedule_ID"] = new SelectList(_context.daily_outpatient_clinic_schedules, "daily_outpatient_clinic_schedule_ID", "daily_outpatient_clinic_schedule_ID", appointment.daily_outpatient_clinic_schedule_ID);
        //    ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", appointment.keeper_ID);
        //    ViewData["pet_ID"] = new SelectList(_context.pets, "pet_ID", "pet_ID", appointment.pet_ID);
        //    return View(appointment);
        //}

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Appointment_ID,pet_ID,keeper_ID,registration_time,daily_outpatient_clinic_schedule_ID,Appointment_status")] Appointment appointment)
        //{
        //    if (id != appointment.Appointment_ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(appointment);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AppointmentExists(appointment.Appointment_ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Appointment_status"] = new SelectList(_context.order_statuses, "ID", "status_name", appointment.Appointment_status);
        //    ViewData["daily_outpatient_clinic_schedule_ID"] = new SelectList(_context.daily_outpatient_clinic_schedules, "daily_outpatient_clinic_schedule_ID", "daily_outpatient_clinic_schedule_ID", appointment.daily_outpatient_clinic_schedule_ID);
        //    ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", appointment.keeper_ID);
        //    ViewData["pet_ID"] = new SelectList(_context.pets, "pet_ID", "pet_ID", appointment.pet_ID);
        //    return View(appointment);
        //}

        // GET: Appointments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.Appointments
        //        .Include(a => a.Appointment_statusNavigation)
        //        .Include(a => a.daily_outpatient_clinic_schedule)
        //        .Include(a => a.keeper)
        //        .Include(a => a.pet)
        //        .FirstOrDefaultAsync(m => m.Appointment_ID == id);
        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(appointment);
        //}

        // POST: Appointments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var appointment = await _context.Appointments.FindAsync(id);
        //    if (appointment != null)
        //    {
        //        _context.Appointments.Remove(appointment);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Appointment_ID == id);
        }
    }
}
