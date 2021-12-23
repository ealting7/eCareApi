using DinkToPdf;
using DinkToPdf.Contracts;
using eCareApi.Context;
using eCareApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

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
            //commented out to see if api is hit....
            new CustomAssemblyLoadContext().LoadUnmanagedLibrary(Path.GetFullPath("./dinktopdf/64/libwkhtmltox.dll"));

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();

            ConfigureCorsService(services);
            ConfigureDatabaseService(services);

            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

            services.AddScoped<IDoctor, DoctorService>();            
            services.AddScoped<IFaxPool, FaxPoolService>();
            services.AddScoped<IIcmsUser, IcmsUserService>();
            services.AddScoped<IMedicalCode, MedicalCodeService>();
            services.AddScoped<IPatient, PatientService>();
            services.AddScoped<IStandard, StandardService>();
            services.AddScoped<IHospital, HospitalService>();
            services.AddScoped<ILab, LabService>();
            services.AddScoped<IBilling, BillingService>();
            services.AddScoped<IUtilizationManagement, UtilizationManagementService>();
            services.AddScoped<IPatientCondition, PatientConditionService>();
            services.AddScoped<IConditionAssessment, ConditionAssessmentService>();
            services.AddScoped<IIcu, IcuService>();
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
            //Database context connection in appsettings.json
            var icmsConnectionData = _configuration.GetValue<string>("icmsDbConnect");
            var emrConnectionData = _configuration.GetValue<string>("emrDbConnect");
            var dataStagingConnectionData = _configuration.GetValue<string>("dataStagingDbConnect");

            services.AddDbContext<IcmsContext>(o =>
            {
                o.UseSqlServer(icmsConnectionData);
            });

            services.AddDbContext<AspNetContext>(o =>
            {
                o.UseSqlServer(emrConnectionData);
            });

            services.AddDbContext<IcmsDataStagingContext>(o =>
            {
                o.UseSqlServer(dataStagingConnectionData);
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
