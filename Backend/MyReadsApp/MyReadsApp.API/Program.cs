using Microsoft.AspNetCore.Identity;
using MyReadsApp.API.Middleware.Exceptions;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Infstructure;
using MyReadsApp.Infstructure.Seeder;
using System.Text.Json.Serialization;
namespace MyReadsApp.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers() 
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // reigister Services
            builder.Services.AddIfstracture(builder.Configuration);
            builder.Services.AddTransient<ExceptionHandeler>();

            var app = builder.Build();

            #region DataSeed
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<Role>>();
                var UserManager = services.GetRequiredService<UserManager<User>>();
                await RoleSeeder.SeedRole(roleManager);
                await UserSeeder.SeedUser(UserManager);
            }
            #endregion
            
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionHandeler>();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
