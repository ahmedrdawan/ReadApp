using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MyReadsApp.Infstructure.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedUser(UserManager<User> userManager)
        {
            var user = new User
            {
                Gender = Gender.Male,
                Id = Guid.NewGuid(),
                UserName = "Admin",
                Email = "admin1@gmail.com",
                BirthDate = new DateTime(2005, 3, 2),
                CreatedAt = DateTime.Now,
                Role = "Admin"
            };
            await userManager.CreateAsync(user, "Admin123@3");
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
