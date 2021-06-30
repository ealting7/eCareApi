using eCareApi.Context;
using eCareApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;


namespace eCareApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            ConfigureCorsService(services);
            ConfigureDatabaseService(services);

            services.AddScoped<IDoctor, DoctorService>();            
            services.AddScoped<IFaxPool, FaxPoolService>();
            services.AddScoped<IIcmsUser, IcmsUserService>();
            services.AddScoped<IMedicalCode, MedicalCodeService>();
            services.AddScoped<IPatient, PatientService>();
            services.AddScoped<IStandard, StandardService>();
            services.AddScoped<IHospital, HospitalService>();
            services.AddScoped<ILab, LabService>();
        }

        private void ConfigureCorsService(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        private void ConfigureDatabaseService(IServiceCollection services)
        {
            //Database context connection
            var connectionString = _configuration["connectionStrings:icmsDbConnectionString"];
            var aspnetConnectionString = _configuration["connectionStrings:aspDbConnectionString"];

            services.AddDbContext<IcmsContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            services.AddDbContext<AspNetContext>(o =>
            {
                o.UseSqlServer(aspnetConnectionString);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
