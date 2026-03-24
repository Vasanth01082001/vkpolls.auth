using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.Application.Contracts.Identity;
using vkpolls.auth.Identity.DbContext;
using vkpolls.auth.Identity.Services;

namespace vkpolls.auth.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VKPollsIdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("VKPollsIdentityConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

                //options.SignIn.RequireConfirmedPhoneNumber = true;
                //options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<VKPollsIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGenerateUserNameService, GenerateUserNameService>();
            services.AddScoped<IAuthConfirmation, AuthConfirmation>();

            return services;
        }
    }
}
