using BangchakStationService.Data;
using BangchakStationService.Models;

namespace BangchakStationService.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly MySQLDbContext _context;
        public UserService(MySQLDbContext context) {
            _context = context;
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.AddAsync(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
