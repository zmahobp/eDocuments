using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Application.Controllers;
[ApiController]
[Route("[controller]")]
public class UserSupportController : ControllerBase
{
    public DatabaseContext Context { get; set; }
    public UserSupportController(DatabaseContext context)
    {
        Context = context;
    }

    [HttpPost("CreateUserSupport")]
    public async Task<ActionResult> CreateUserSupport([FromForm] string UserSupportJson)
    {
        try
        {
            var UserSupport = JsonConvert.DeserializeObject<UserSupport>(UserSupportJson);
            var support = await Context.UserSupports.FirstOrDefaultAsync(u => u.Username == UserSupport.Username);
            var user = await Context.RegularUsers.FirstOrDefaultAsync(u => u.Username == UserSupport.Username);
            if (support != null || user != null)
                return BadRequest("A user with this username already exists.");

            UserSupport.SetSupport();
            UserSupport.SetHash();
            UserSupport.GenerateSecretKey();
            await Context.UserSupports.AddAsync(UserSupport);
            await Context.SaveChangesAsync();
            return Ok("Support user account successfully registered.");
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
        var user = await Context.UserSupports.FirstOrDefaultAsync(u => u.Username == UserSupport.Username);

        if (user != null && user.Authenticate(UserSupport.Password))
        {
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        else
        {
            var errorResponse = new
            {
                Message = "Invalid username or password.",
                UserSupportJson = UserSupportJson,
                User = user
            };
            return BadRequest(errorResponse);
        }
    }

    private string GenerateJwtToken(UserSupport user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(user.SecretKey);

        var claims = new List<Claim>
        {
            new Claim("accountType", "UserSupport"),
            new Claim("userId", user.ID.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim("username", user.Username),
            new Claim("email", user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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