using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using InvoiceGenerator.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InvoiceGenerator.Models;
using AutoMapper;
using InvoiceGenerator.AutoMapperProfile;
using Microsoft.Extensions.Logging;
using SendGrid;

namespace InvoiceGenerator
{
    public class Startup
    {
        private AppSettings Settings = new AppSettings();
        private readonly IWebHostEnvironment _env;
        private readonly AppSettings _settings = new AppSettings();
        private ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration,IWebHostEnvironment Env)
        {
            Configuration = configuration;
            _env = Env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            if (_env.IsDevelopment())
            {
                // note these settings are contained in secrets.json to avoid being included in a source control repository
                // select manage user secrets to view\edit the keys
                _settings.DatabaseConnection = Configuration["TestDBConnection"];
                _settings.SendGridUser = Configuration["SendGridUser"];
                _settings.SendGridKey = Configuration["SendGridKey"];
                _settings.Test = Configuration["Test"];
            }

            if (_env.IsProduction())
            {
                // note these settings are contained in secrets.json to avoid being included in a source control repository
                // select manage user secrets to view\edit the keys
                _settings.DatabaseConnection = Configuration["TestDBConnection"];
                _settings.SendGridUser = Configuration["SendGridUser"];
                _settings.SendGridKey = Configuration["SendGridKey"];
                _settings.Test = Configuration["Test"];
            }

           
           

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_settings.DatabaseConnection));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(dbconn));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            var PostEmail = new SendGridClient(_settings.SendGridKey);
            services.AddSingleton<SendGridClient>(PostEmail);
            services.AddSingleton<AppSettings>(_settings);
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
