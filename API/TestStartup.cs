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

namespace SigortaDefterimV2API
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {

        }

        public override void SetupDatabase(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        }
    }
}
