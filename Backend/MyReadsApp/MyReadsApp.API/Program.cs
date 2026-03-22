using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpLogging;
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

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers() 
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.RequestMethod |
                                        HttpLoggingFields.RequestPath |
                                        HttpLoggingFields.ResponseStatusCode |
                                        HttpLoggingFields.Duration;
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

            app.UseCors("AllowAll");
            app.UseMiddleware<ExceptionHandeler>();
            app.UseHttpLogging();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
