using Lapka.BLL.Services;
using Lapka.BLL.Services.Animals;
using Lapka.DAL.Repository;
using Lapka.SharedModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lapka.API.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void Configure(
            IConfiguration configuration,
            IServiceCollection services)
        {
            // configure DI for application services

            // DAL repositories
            services.AddScoped<UsersRepository>();

            // BLL Services
            services.AddScoped<BlobService>();
            services.AddScoped<UserService>();
            services.AddScoped<AnimalsService>();
            //services.AddScoped<ShelterService>();

            // SendGrid email sender service
            //services.AddScoped<EmailSenderService>();
            services.Configure<EmailSenderOptions>(
                configuration.GetSection(EmailSenderOptions.SectionName));
        }
    }
}

