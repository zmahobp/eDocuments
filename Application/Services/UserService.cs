using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;

        public UserService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterUserAsync(RegularUser user)
        {
            var existingUser = await _context.RegularUsers.FirstOrDefaultAsync(u => u.Username == user.Username);
            var support = await _context.UserSupports.FirstOrDefaultAsync(u => u.Username == user.Username);
            
            if (existingUser != null || support != null)
                return "A user with this username already exists.";

            try
            {
                user.OfficialPerson = false;
                user.SetHash(user.Password);
                user.GenerateSecretKey();
                await _context.RegularUsers.AddAsync(user);
                await _context.SaveChangesAsync();
                return "User successfully registered.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> LoginUserAsync(string username, string password)
        {
            var user = await _context.RegularUsers.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && user.Authenticate(password))
            {
                return GenerateJwtToken(user, username);
            }
            else
            {
                return "Invalid username or password.";
            }
        }

        public async Task<string> RegisterSupportAsync(UserSupport user)
        {
            var existingUser = await _context.UserSupports.FirstOrDefaultAsync(u => u.Username == user.Username);
            var regularUser = await _context.RegularUsers.FirstOrDefaultAsync(u => u.Username == user.Username);
            
            if (existingUser != null || regularUser != null)
                return "A user with this username already exists.";

            try
            {
                user.IsAdmin = false;
                user.SetHash(user.Password);
                await _context.UserSupports.AddAsync(user);
                await _context.SaveChangesAsync();
                return "User support successfully registered.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> LoginSupportAsync(string username, string password)
        {
            var user = await _context.UserSupports.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && user.Authenticate(password))
            {
                return GenerateJwtTokenForSupport(user, username);
            }
            else
            {
                return "Invalid username or password.";
            }
        }

        public async Task<RegularUser> GetUserByIdAsync(int userId)
        {
            return await _context.RegularUsers.FindAsync(userId);
        }

        public async Task<UserSupport> GetSupportByIdAsync(int supportId)
        {
            return await _context.UserSupports.FindAsync(supportId);
        }

        private string GenerateJwtToken(RegularUser user, string username)
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
                new Claim("accountType", "user")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtTokenForSupport(UserSupport user, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(user.SecretKey);

            var claims = new List<Claim>
            {
                new Claim("userId", user.ID.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("username", user.Username),
                new Claim("email", user.Email),
                new Claim("isAdmin", user.IsAdmin.ToString()),
                new Claim("accountType", "support")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}