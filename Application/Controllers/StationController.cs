using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application.Controllers;
[ApiController]
[Route("[controller]")]
public class StationController : ControllerBase
{
    public DatabaseContext Context { get; set; }
    public StationController(DatabaseContext context)
    {
        Context = context;
    }

    [HttpPost("AddStation")]
    public async Task<ActionResult> AddStation([FromBody] Station station)
    {
        if (station == null)
            return BadRequest("Invalid data");
        bool exists = false;
        foreach (Station st in Context.Stations)
        {
            if (st.City == station.City && st.Municipality == station.Municipality)
            {
                exists = true;
            }
        }
        if (exists)
            return BadRequest("A station already exists in this municipality.");
        try
        {
            station.Appointments = new List<Appointment>();
            await Context.Stations.AddAsync(station);
            await Context.SaveChangesAsync();
            return Ok("Station successfully added.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetStations")]
    public List<Station> GetStations()
    {
        List<Station> stations = new List<Station>();
        foreach (var s in Context.Stations)
        {
            stations.Add(s);
        }
        return stations;
    }

    [HttpGet("SearchStations")]
    public async Task<ActionResult> SearchStations(string city, string municipality)
    {
        try
        {
            var station = await Context.Stations
                .Include(st => st.Appointments)
                .ThenInclude(app => app.User)
                .Where(st => st.City == city && st.Municipality == municipality)
                .Select(st => new
                {
                    ID = st.ID,
                    Name = st.Name,
                    PhoneNumber = st.PhoneNumber,
                    City = st.City,
                    Municipality = st.Municipality,
                    Street = st.Street,
                    Number = st.Number,
                    Appointments = st.Appointments
                        .Where(app => app.DateTime > DateTime.Now)
                        .Select(app => new
                        {
                            ID = app.ID,
                            DateTime = app.DateTime,
                            StationID = st.ID,
                            UserID = app.User.ID,
                            Description = app.Description,
                            Status = app.Status
                        }).OrderByDescending(app => app.DateTime)
                        .ToList()
                })
                .FirstOrDefaultAsync();
            if (station == null)
                return BadRequest("Station not found.");
            return Ok(station);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}