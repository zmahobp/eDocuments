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
public class RegularUserController : ControllerBase
{
    public DatabaseContext Context { get; set; }
    public RegularUserController(DatabaseContext context)
    {
        Context = context;
    }

    [HttpPost("RegisterRegularUser")]
    public async Task<ActionResult> RegisterRegularUser(IFormFile photo, [FromForm] string userJson)
    {
        var user = JsonConvert.DeserializeObject<RegularUser>(userJson);
        var existingUser = await Context.RegularUsers.FirstOrDefaultAsync(u => u.Username == user.Username);
        var support = await Context.UserSupports.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null || support != null)
            return BadRequest("A user with this username already exists.");

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
            user.SetHash(user.Password);
            user.GenerateSecretKey();
            await Context.RegularUsers.AddAsync(user);
            await Context.SaveChangesAsync();
            return Ok("User successfully registered.");
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
        var user = await Context.RegularUsers.FirstOrDefaultAsync(u => u.Username == userInput.Username);

        if (user != null && user.Authenticate(userInput.Password))
        {
            var token = GenerateJwtToken(user, userInput.Password);
            return Ok(new { Token = token });
        }
        else
        {
            return BadRequest("Invalid username or password.");
        }
    }

    private string GenerateJwtToken(RegularUser user, string password)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(user.SecretKey);

        var claims = new List<Claim>
        {
            new Claim("userId", user.ID.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("parentName", user.ParentName),
            new Claim("jmbg", user.JMBG),
            new Claim("lastName", user.LastName),
            new Claim("username", user.Username),
            new Claim("email", user.Email),
            new Claim("city", user.City),
            new Claim("municipality", user.Municipality),
            new Claim("street", user.Street),
            new Claim("number", user.Number),
            new Claim("phone", user.Phone),
            new Claim("birthDate", user.BirthDate.ToString("o")),
            new Claim("birthPlace", user.BirthPlace),
            new Claim("gender", user.Gender.ToString()),
            new Claim("officialPerson", user.OfficialPerson.ToString()),
            new Claim("accountType", "user"),
            new Claim("password", password)
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