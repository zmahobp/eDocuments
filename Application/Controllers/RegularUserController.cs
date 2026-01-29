using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;

namespace Application.Controllers;
[ApiController]
[Route("[controller]")]
public class RegularUserController : ControllerBase
{
    private readonly IUserService _userService;

    public RegularUserController(IUserService userService)
    {
        _userService = userService;
    }

[HttpPost("RegisterRegularUser")]
    public async Task<ActionResult> RegisterRegularUser(IFormFile photo, [FromForm] string userJson)
    {
        var user = JsonConvert.DeserializeObject<RegularUser>(userJson);

        try
        {
            if (photo != null && photo.Length > 0)
            {
                var photoName = $"{user.FirstName}{user.LastName}{user.JMBG}{Path.GetExtension(photo.FileName)}";
                var photoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos", photoName);

                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                user.Photo = photoPath;
            }

            user.OfficialPerson = false;
            user.GenerateSecretKey();
            
            var result = await _userService.RegisterUserAsync(user);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

[HttpPost("LoginUser")]
    public async Task<ActionResult> LoginUser([FromForm] string userJson)
    {
        var userInput = JsonConvert.DeserializeObject<RegularUser>(userJson);
        var result = await _userService.LoginUserAsync(userInput.Username, userInput.Password);

        if (!result.Contains("Invalid"))
        {
            return Ok(new { Token = result });
        }
        else
        {
            return BadRequest(result);
        }
}

    [HttpPut("UpdateRegularUser")]
    public async Task<ActionResult> UpdateRegularUser(int id, RegularUser updatedUser)
    {
        var user = await Context.RegularUsers.FindAsync(id);
        if (updatedUser == null || user == null || id == 0)
        {
            return BadRequest("Invalid data. (User ID is required).");
        }

        try
        {
            user.FirstName = updatedUser.FirstName;
            user.ParentName = updatedUser.ParentName;
            user.JMBG = updatedUser.JMBG;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.City = updatedUser.City;
            user.Municipality = updatedUser.Municipality;
            user.Street = updatedUser.Street;
            user.Number = updatedUser.Number;
            user.Phone = updatedUser.Phone;
            user.BirthDate = updatedUser.BirthDate;
            user.BirthPlace = updatedUser.BirthPlace;
            user.Gender = updatedUser.Gender;
            user.Username = updatedUser.Username;

            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("UpdatePhoto")]
    public async Task<ActionResult> UpdatePhoto(int userId, IFormFile photo)
    {
        try
        {
            var user = await Context.RegularUsers.FindAsync(userId);
            if (photo != null && photo.Length > 0)
            {
                var photoName = $"{user.FirstName}{user.LastName}{user.JMBG}{Path.GetExtension(photo.FileName)}";
                var photoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos", photoName);

                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                user.Photo = photoPath;
            }
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();
            return Ok("User photo successfully updated.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("UpdateUsername")]
    public async Task<ActionResult> UpdateUsername(int userId, string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("Invalid input.");
            var existing = await Context.RegularUsers.FirstOrDefaultAsync(l => l.Username == username);
            if (existing != null)
                return BadRequest("A user with this username already exists.");

            var user = await Context.RegularUsers.FindAsync(userId);
            user.Username = username;
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();
            return Ok("Username successfully updated.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("UpdatePassword")]
    public async Task<ActionResult> UpdatePassword(int userId, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
                return BadRequest("Invalid input.");
            var user = await Context.RegularUsers.FindAsync(userId);
            user.SetHash(password);
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();
            return Ok("Password successfully updated.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // documentName values: "WeaponPermit", "IDCard", "Passport", "VehicleRegistration", "DrivingLicense"
    [HttpGet("CheckDocument")]
    public async Task<ActionResult> CheckDocument(int userId, string documentName)
    {
        var user = await Context.RegularUsers
            .Include(u => u.IDCard)
            .Include(u => u.Passport)
            .Include(u => u.DrivingLicense)
            .Include(u => u.VehicleRegistration)
            .Include(u => u.WeaponPermit)
            .FirstOrDefaultAsync(u => u.ID == userId);

        switch (documentName)
        {
            case "IDCard":
                return Ok(new { hasIDCard = user.IDCard != null });
            case "Passport":
                return Ok(new { hasPassport = user.Passport != null });
            case "WeaponPermit":
                return Ok(new { hasWeaponPermit = user.WeaponPermit != null });
            case "DrivingLicense":
                return Ok(new { hasDrivingLicense = user.DrivingLicense != null });
            case "VehicleRegistration":
                return Ok(new { hasVehicleRegistration = user.VehicleRegistration != null });
            default:
                return BadRequest("Invalid class name.");
        }
    }

    [HttpDelete("DeleteRegularUser")]
    public async Task<ActionResult> DeleteRegularUser(int userId)
    {
        try
        {
            var user = await Context.RegularUsers.FindAsync(userId);
            Context.RegularUsers.Remove(user);
            await Context.SaveChangesAsync();
            return Ok("User successfully deleted.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}