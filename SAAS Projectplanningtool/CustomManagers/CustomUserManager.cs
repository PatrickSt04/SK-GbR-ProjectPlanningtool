using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using System.Threading;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class CustomUserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public CustomUserManager(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Employee?> GetEmployeeAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);
            return await GetEmployeeAsync(user);
        }

        public async Task<IdentityUser?> CreateIdentityUser(string email, string roleId, RoleManager<IdentityRole> roleManager)
        {
            await _semaphore.WaitAsync(); // Verhindert parallele Zugriffe
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await EnsureEmailIsUniqueAsync(email);

                    var initialPassword = await CreateNewIdentityUserAsync(email, roleId, roleManager);

                    await transaction.CommitAsync();
                    return await _userManager.FindByEmailAsync(email);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task EnsureEmailIsUniqueAsync(string email)
        {
            if (await EmailAlreadyExistsAsync(email))
            {
                throw new Exception("Der anzulegende Benutzer existiert bereits.");
            }
        }


        private async Task<bool> EmailAlreadyExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        private async Task<string> CreateNewIdentityUserAsync(string email, string roleId, RoleManager<IdentityRole> roleManager)
        {
            var initialPassword = "Password123!";
            var userToBeCreated = new IdentityUser { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(userToBeCreated, initialPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Fehler beim Schreiben in die Datenbank");
            }

            await AddRoleToNewUserAsync(roleId, userToBeCreated, roleManager);
            return initialPassword;
        }

        private async Task AddRoleToNewUserAsync(string roleId, IdentityUser userWithoutRole, RoleManager<IdentityRole> roleManager)
        {
            var roleToAddToUser = await roleManager.FindByIdAsync(roleId);
            if (roleToAddToUser?.Name == null)
            {
                throw new Exception("Fehler bei der Rollenzuweisung");
            }
            await _userManager.AddToRoleAsync(userWithoutRole, roleToAddToUser.Name);
        }

        private async Task<Employee?> GetEmployeeAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new Exception("Benutzer konnte nicht gefunden werden");
            }

            var employee = await _context.Employee
                .Include(cu => cu.Company)
                .FirstOrDefaultAsync(cu => cu.IdentityUserId == user.Id);

            return employee ?? throw new Exception("Benutzer konnte nicht gefunden werden");
        }

        private async Task<IdentityUser?> FindUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}