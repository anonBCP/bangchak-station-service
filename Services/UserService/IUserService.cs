using BangchakStationService.Models;

namespace BangchakStationService.Services.UserService
{
    public interface IUserService
    {
        Task CreateUserAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
