using Microsoft.EntityFrameworkCore;
using Models;

namespace Services
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(RegularUser user);
        Task<string> LoginUserAsync(string username, string password);
        Task<string> RegisterSupportAsync(UserSupport user);
        Task<string> LoginSupportAsync(string username, string password);
        Task<RegularUser> GetUserByIdAsync(int userId);
        Task<UserSupport> GetSupportByIdAsync(int supportId);
    }
}