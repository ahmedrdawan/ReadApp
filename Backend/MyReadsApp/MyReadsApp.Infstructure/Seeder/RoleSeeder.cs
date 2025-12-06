using MyReadsApp.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedRole(RoleManager<Role> roleManger)
        {
            List<string> roles = ["Admin", "User"];
            foreach (var role in roles)
            {
                Role? result = await roleManger.FindByNameAsync(role);
                var newRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                };
                if (result == null)
                    await roleManger.CreateAsync(newRole);
            }
        }
    }
}
