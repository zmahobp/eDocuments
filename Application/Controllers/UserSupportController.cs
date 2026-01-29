using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;

namespace Application.Controllers;
[ApiController]
[Route("[controller]")]
public class UserSupportController : ControllerBase
{
    private readonly IUserService _userService;

    public UserSupportController(IUserService userService)
    {
        _userService = userService;
    }

[HttpPost("CreateUserSupport")]
    public async Task<ActionResult> CreateUserSupport([FromForm] string UserSupportJson)
    {
        try
        {
            var UserSupport = JsonConvert.DeserializeObject<UserSupport>(UserSupportJson);
            UserSupport.SetSupport();
            UserSupport.GenerateSecretKey();
            
            var result = await _userService.RegisterSupportAsync(UserSupport);
            
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

    [HttpDelete("DeleteUserSupport")]
    public async Task<ActionResult> DeleteUserSupport(int UserSupportId)
    {
        try
        {
            var UserSupport = await Context.UserSupports.FindAsync(UserSupportId);
            Context.UserSupports.Remove(UserSupport);
            await Context.SaveChangesAsync();
            return Ok("Support user account successfully deleted.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

[HttpPost("LoginUserSupport")]
    public async Task<ActionResult> LoginUserSupport([FromForm] string UserSupportJson)
    {
        var UserSupport = JsonConvert.DeserializeObject<UserSupport>(UserSupportJson);
        var result = await _userService.LoginSupportAsync(UserSupport.Username, UserSupport.Password);

        if (!result.Contains("Invalid"))
        {
            return Ok(new { Token = result });
        }
        else
        {
            return BadRequest(result);
        }
}

    [HttpGet("CheckOfficialPerson")]
    public async Task<ActionResult<bool>> CheckOfficialPerson(string jmbg)
    {
        try
        {
            var user = await Context.RegularUsers.FirstOrDefaultAsync(u => u.JMBG == jmbg);

            if (user != null && user.OfficialPerson)
            {
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("SetOfficialPerson")]
    public async Task<ActionResult> SetOfficialPerson(string jmbg)
    {
        try
        {
            var user = await Context.RegularUsers.FirstOrDefaultAsync(u => u.JMBG == jmbg);

            if (user == null)
            {
                return NotFound("User with the given JMBG was not found.");
            }

            if (user.OfficialPerson)
            {
                return BadRequest("User is already set as an official person.");
            }

            user.SetOfficialPerson();
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Official person status successfully set.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("RemoveOfficialPerson")]
    public async Task<ActionResult> RemoveOfficialPerson(string jmbg)
    {
        try
        {
            var user = await Context.RegularUsers.FirstOrDefaultAsync(u => u.JMBG == jmbg);

            if (user == null)
            {
                return NotFound("User with the given JMBG was not found.");
            }

            if (!user.OfficialPerson)
            {
                return BadRequest("User is not set as an official person.");
            }

            user.RemoveOfficialPerson();
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Official person status successfully removed.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("SearchByJMBG")]
    public async Task<ActionResult> SearchByJMBG(string JMBG)
    {
        try
        {
            var user = await Context.RegularUsers
                                    .Include(u => u.IDCard)
                                    .Include(u => u.Passport)
                                    .Include(u => u.DrivingLicense)
                                    .Include(u => u.VehicleRegistration)
                                    .Include(u => u.WeaponPermit)
                                    .Include(u => u.Appointments)
                                    .FirstOrDefaultAsync(u => u.JMBG == JMBG);
            if (user != null)
            {
                user.SecretKey = "";
                user.HashPassword = "";
                return Ok(user);
            }
            else
            {
                return BadRequest("User with this JMBG does not exist in the database.");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("UpdatePhoto")]
    public async Task<ActionResult> UpdatePhoto(string JMBG, IFormFile photo)
    {
        try
        {
            var user = await Context.RegularUsers.FirstOrDefaultAsync(x => x.JMBG == JMBG);
            if (user == null)
            {
                return NotFound("User not found.");
            }

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
}