using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SyllabusZip.Common.Data;

namespace SyllabusZip
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Functionality to load environment variables without running in Visual Studio
            DotEnv.Load();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    // Based on https://dusted.codes/dotenv-in-dotnet
    public static class DotEnv
    {
        public static void Load(string filePath = null)
        {
            if (filePath is null)
            {
                var root = Directory.GetCurrentDirectory();
                filePath = Path.Combine(root, ".env");
            }

            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
