using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyllabusZip.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

[assembly:FunctionsStartup(typeof(SyllabusZipSort.Startup))]

namespace SyllabusZipSort
{
    class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
            
        }
    }
}
