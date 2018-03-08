using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneSystemManagement.Core.Extensions;
using OneSystemManagement.Core.Responses.ApiResponses;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OneSystemAdminApi.Core.DataLayer;

namespace OneSystemManagement
{
    public class Startup
    {
        public static string WebRootPath { get; private set; }

        public static string MapPath(string path, string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = WebRootPath;
            }

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(basePath, path);
        }
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(a => a.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddJsonOptions(a => a.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddMvc();
            services.AddEntityFrameworkSqlServer().AddDbContext<OneSystemDbContext>();
            services.AddScoped<IEntityMapper, OneSystemEntityMapper>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAreaService, AreaService>();

            services.AddOptions();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddSingleton(Configuration);
           
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "oneoffice.vn",
                        ValidAudience = "oneoffice.vn",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
                    };
                });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/SystemAdmin/login");
                        options.AccessDeniedPath = new PathString("/SystemAdmin/login");
                        options.ReturnUrlParameter = "returnUrl";
                    });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "One System API",
                    Description = "One System Admin",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Tham Davies", Email = "thamdv96@gmail.com", Url = "https://facebook.com/thamdavies.dev" },
                    License = new License { Name = "Use under LICX", Url = "https://example.com/license" }
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = _hostingEnvironment.WebRootPath;
                var xmlPath = Path.Combine(basePath, "OneSystemAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });
            
            services.AddAutoMapper();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                /* For example, in the below code instead of having the folder name as 'Areas', it has been changed to 'admin'.
                    / Areas /< Area - Name >/ Views /< Controller - Name >/< Action - Name >.cshtml
                   / Areas /< Area - Name >/ Views / Shared /< Action - Name >.cshtml
                   / Views / Shared /< Action - Name >.cshtml
               */
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            return services.RegisterService(Configuration, _hostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OneSystem API V1");
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                );

                routes.MapRoute("login", "systemadmin/login",
                    defaults: new { controller = "Account", action = "Login", area = "SystemAdmin" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            WebRootPath = env.WebRootPath;
        }
    }
}
