using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy; 
using Microsoft.EntityFrameworkCore;
using DeltaX.Auth.IdentityServer.Data;
using DeltaX.Auth.IdentityServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeltaX.Auth.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
             
             services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddEntityFrameworkStores<ApplicationDbContext>();
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();


            // services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
           var builder = services.AddIdentityServer(options =>
           {
               options.Events.RaiseErrorEvents = true;
               options.Events.RaiseInformationEvents = true;
               options.Events.RaiseFailureEvents = true;
               options.Events.RaiseSuccessEvents = true;
           
               // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
               options.EmitStaticAudienceClaim = true;
           })
              .AddInMemoryIdentityResources(Config.IdentityResources)
              .AddInMemoryApiScopes(Config.ApiScopes)
              .AddInMemoryClients(Config.Clients)
              // .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
              .AddAspNetIdentity<ApplicationUser>();
           // not recommended for production - you need to store your key material somewhere secure
           builder.AddDeveloperSigningCredential();


            services.AddAuthentication()
                 // .AddGoogle(options =>
                 // {
                 //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                 // 
                 //     // register your IdentityServer with Google at https://console.developers.google.com
                 //     // enable the Google+ API
                 //     // set the redirect URI to https://localhost:5011/signin-google
                 //     options.ClientId = "copy client ID from Google here";
                 //     options.ClientSecret = "copy client secret from Google here";
                 // });
                // .AddIdentityServerJwt()
                ;

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
