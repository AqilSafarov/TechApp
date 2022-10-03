using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop
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
            services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"))).AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();
            
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Member_Auth";
                options.DefaultSignInScheme = "Member_Auth";
            })
                .AddCookie("Member_Auth", options =>
             {
                 options.LoginPath = "/account/login";
                 options.AccessDeniedPath = "/account/login";

             })
            .AddCookie("Admin_Auth", options =>
            {
                options.LoginPath = "/manage/account/login";
                options.AccessDeniedPath = "/manage/account/login";
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "Member_Auth";
            //    options.DefaultSignInScheme = "Member_Auth";
            //}).AddCookie("Member_Auth", options =>
            //{
            //    options.LoginPath = "/account/login";
            //    options.AccessDeniedPath = "/account/login";
            //}).AddCookie("Admin_Auth", options =>
            //{
            //    options.LoginPath = "/manage/account/login";
            //    options.AccessDeniedPath = "/manage/account/login";
            //});


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.Use(async (context, next) =>
            {
                var area = context.Request.RouteValues["Area"];
                string schem = null;

                if (area==null)
                {
                    foreach (var item in context.Request.Cookies)
                    {
                        if (item.Key.Contains("Member_Auth"))
                        {
                            schem = "Member_Auth";
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var item in context.Request.Cookies)
                    {
                        if (item.Key.Contains("Admin_Auth"))
                        {
                            schem = "Admin_Auth";
                            break;
                        }
                    }
                }
                if (schem!=null)
                {
                    var result=await context.AuthenticateAsync(schem);

                    if (result.Succeeded)
                    {
                        context.User = result.Principal;
                    }
                   
                }
                await next();
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                  );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
