using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SigortaDefterimV2API.Helpers;
using SigortaDefterimV2API.Models;
using SigortaDefterimV2API.Services;
using Microsoft.OpenApi.Models;
using SigortaDefterimV2API.Models.Examples;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using SigortaDefterimV2API.Hosted;

namespace SigortaDefterimV2API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void SetupDatabase(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x =>
            {
                x.UseSqlServer(Configuration.GetConnectionString("Connection"));
            });
            services.AddDbContext<API.Areas.MobilApi.Models.DataContext>(x =>
            {
                x.UseSqlServer(Configuration.GetConnectionString("Connection"));
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddCors(options =>
            //     options.AddPolicy("myclients", builder =>
            //        builder.WithOrigins("https://localhost:44346/").AllowAnyMethod().AllowAnyHeader()));
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            SetupDatabase(services);

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            //services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddSingleton<AppSettings>();

            services.AddScoped<UserService>();
            services.AddScoped<MobileAppService>();
            services.AddScoped<PolicyService>();
            services.AddScoped<CarService>();
            services.AddScoped<API.Areas.MobilApi.Services.SmsService>();
            services.AddHostedService<MobileSmsCleanupHostedService>();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "SigortaDefterimV2 Web API"
                });
                c.ExampleFilters();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });

            services.AddSwaggerExamplesFromAssemblyOf<LoginRequestExample>();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Damageimages")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), "Damageimages")),
                    RequestPath = "/Damageimages"
                });
            }

            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Userphoto")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "Userphoto")),
                    RequestPath = "/Userphoto"
                });
            }
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Pdf")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
           Path.Combine(Directory.GetCurrentDirectory(), "Pdf")),
                    RequestPath = "/Pdf"
                });
            }
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Sound")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "Sound")),
                    RequestPath = "/Sound"
                });
            }
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Images")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                    RequestPath = "/Images"
                });
            }
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "assets")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
              Path.Combine(Directory.GetCurrentDirectory(), "assets")),
                    RequestPath = "/assets"
                });

            }
            if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "CompanyFiles")))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
              Path.Combine(Directory.GetCurrentDirectory(), "CompanyFiles")),
                    RequestPath = "/CompanyFiles"
                });
            }

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                    name: "MobilApi",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SigortaDefterimV2 API");
            });
        }
    }
}
