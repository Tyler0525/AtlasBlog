using AtlasBlog.Data;
using AtlasBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog.Services
{
    public class DataService
    {
        //Calling a method or instruction that executes the migrations
        readonly ApplicationDbContext _dbcontext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbcontext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbcontext = dbcontext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SetupDbAsync()
        {
            //Run the migrations async
           await _dbcontext.Database.MigrateAsync();

            //Add a few Roles into the system
            await SeedRolesAsync();


            //Add a few Users into the system
            await SeedUsersAsync();    
        }

        private async Task SeedUsersAsync()
        {
            BlogUser adminUser = new()
            {
                UserName = "Tyler_taylor05@hotmail.com",
                Email = "Tyler_taylor05@hotmail.com",
                FirstName = "Tyler",
                LastName = "Taylor",
                DisplayName = "Tyler_0525",
                PhoneNumber = "3369785748",
                EmailConfirmed = true,
            };

            BlogUser modUser = new()
            {
                UserName = "DrewRussell@mailinator.com",
                Email = "DrewRussell@mailinator.com",
                FirstName = "Drew",
                LastName = "Russell",
                DisplayName = "Prof",
                PhoneNumber = "3365484665",
                EmailConfirmed = true,
            };

            try
            {
                if  (await _userManager.FindByEmailAsync(adminUser.Email) is null)
                {
                   await _userManager.CreateAsync(adminUser, "Psalms910!");
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                    
                }

                if (await _userManager.FindByEmailAsync(modUser.Email) is null)
                {
                    await _userManager.CreateAsync(modUser, "Abc&123");
                    await _userManager.AddToRoleAsync(modUser, "Moderator");
                    
                }
            }
            catch (Exception ex)
            {
               
            }
        }
        private async Task SeedRolesAsync()
        {
            if (_roleManager.Roles.Any())
            {
                return;
            }
            await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            await _roleManager.CreateAsync(new IdentityRole("Moderator"));
        }
    }
}
