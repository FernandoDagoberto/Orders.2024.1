using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Entities;

namespace Orders.Backend.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManeger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersRepository(DataContext context, UserManager<User> userManeger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManeger = userManeger;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManeger.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManeger.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users
                  .Include(u => u.City!)
                  .ThenInclude(c => c.State!)
                  .ThenInclude(s => s.Country)
                  .FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManeger.IsInRoleAsync(user, roleName);
        }
    }
}