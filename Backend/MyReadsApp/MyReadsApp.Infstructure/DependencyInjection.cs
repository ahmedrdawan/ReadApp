using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyReadsApp.Core.AppSetting;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services;
using MyReadsApp.Infstructure.Services.Account;
using System.Text;

namespace MyReadsApp.Infstructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddIfstracture(
            this IServiceCollection services,
            IConfiguration configration)
        {
            services.AddDatabase(configration);
            services.AddIdentitySystem();
            services.AddJwtAuthentication(configration);
            services.AddScopedServices();
            return services;
        }


        //----------------------------------------------------
        // Database
        //----------------------------------------------------
        private static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configration.GetConnectionString("default"),
                b=>b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            return services;
        }

        //----------------------------------------------------
        // Identity
        //----------------------------------------------------
        private static IServiceCollection AddIdentitySystem(
            this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options=>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();

            return services;
        }

        // -----------------------------------------------------------
        // JWT
        // -----------------------------------------------------------
        private static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings section missing.");

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        // -----------------------------------------------------------
        // Services Scope
        // -----------------------------------------------------------
        private static IServiceCollection AddScopedServices(
           this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAuthorServices, AuthorServices>();
            services.AddScoped<IUserAuthServices, UserAuthServices>();
            services.AddScoped<IBookServices, BookServices>();
            services.AddScoped<IPostServices, PostServices>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<ICommentServises, CommentServices>();
            services.AddScoped<IFaviroteBookServices, FaviroteBookServices>();


            services.AddScoped<IJwtTokenServices, JwtTokenServices>();
            services.AddScoped<IEmailservices, EmailServices>();
            //services.AddSingleton<IEmailservices, EmailServices>();
            return services;
        }

    }
}
