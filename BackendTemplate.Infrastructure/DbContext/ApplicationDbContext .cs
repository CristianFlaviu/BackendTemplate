using BackendTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendTemplate.Infrastructure.DbContext
{
    class ApplicationDbContext : IdentityDbContext<UserEntity>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserEntity>(entity =>
            {
            });
        }

        public static async Task SeedData(IServiceProvider serviceProvider, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Admin role
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
            }

            // Seed Admin user
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var user = new UserEntity
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
