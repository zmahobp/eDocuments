using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Application.Controllers;
[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    public DatabaseContext Context { get; set; }
    public AppointmentController(DatabaseContext context)
    {
        Context = context;
    }

    [HttpGet("GetAppointment")]
    public async Task<ActionResult> GetAppointment(int userId)
    {
        try
        {
            var user = await Context.RegularUsers
                            .Include(k => k.Appointments)
                            .ThenInclude(t => t.Station)
                            .Where(k => k.ID == userId)
                            .Select(k => new
                            {
                                LatestAppointment = k.Appointments
                                    .OrderByDescending(t => t.DateTime)
                                    .Select(t => new
                                    {
                                        ID = t.ID,
                                        userId = userId,
                                        DateTime = t.DateTime,
                                        StationName = t.Station.Name,
                                        CityMunicipality = t.Station.City + ", " + t.Station.Municipality,
                                        StreetNumber = t.Station.Street + ", " + t.Station.Number,
                                        Description = t.Description,
                                        Status = t.Status
                                    })
                                    .FirstOrDefault()
                            })
                            .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("User not found.");
            }
            if (user.LatestAppointment != null && user.LatestAppointment.DateTime > DateTime.Now)
            {
                return Ok(user.LatestAppointment);
            }
            else
                return BadRequest();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("CreateAppointment")]
    public async Task<ActionResult> CreateAppointment([FromForm] string dataJson) // dataJson { userId, stationId, description, dateTime }
    {
        try
        {
            var appointmentData = JsonConvert.DeserializeObject<AppointmentData>(dataJson);

            if (appointmentData == null)
                return BadRequest("Invalid data.");

            var user = await Context.RegularUsers.FindAsync(appointmentData.UserId);
            var station = await Context.Stations.FindAsync(appointmentData.StationId);

            if (user == null || station == null)
                return BadRequest("Station or user does not exist in the database.");

            Appointment appointment = new Appointment
            {
                DateTime = appointmentData.DateTime,
                Station = station,
                User = user,
                Description = appointmentData.Description,
                Status = "scheduled"
            };

            await Context.Appointments.AddAsync(appointment);
            await Context.SaveChangesAsync();
            var returnAppointment = new
            {
                id = appointment.ID,
                dateTime = appointment.DateTime,
                stationId = appointment.Station.ID,
                userId = appointment.User.ID,
                description = appointment.Description,
                status = appointment.Status
            };
            return Ok(returnAppointment);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("CancelAppointment")]
    public async Task<ActionResult> CancelAppointment(int appointmentId)
    {
        try
        {
            var appointment = await Context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return BadRequest("Invalid data.");

            Context.Appointments.Remove(appointment);
            await Context.SaveChangesAsync();
            return Ok("Appointment successfully cancelled.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("UpdateAppointment")]
    public async Task<ActionResult> UpdateAppointment([FromForm] string dataJson)
    {
        try
        {
            var appointmentJson = JsonConvert.DeserializeObject<Appointment>(dataJson);
            var appointment = await Context.Appointments.FindAsync(appointmentJson.ID);

            if (appointment == null || appointmentJson == null)
                return BadRequest("Invalid data.");

            appointment.Description = appointmentJson.Description;
            appointment.Status = appointmentJson.Status;

            Context.Appointments.Update(appointment);
            await Context.SaveChangesAsync();
            return Ok("Appointment successfully updated.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    public class AppointmentData
    {
        public int UserId { get; set; }
        public int StationId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
    }
}