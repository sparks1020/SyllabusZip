using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SyllabusZip.Services;

namespace SyllabusZip.Tests.Services
{
    /// <summary>
    ///     The Canvas API tests use user-secrets to get the information about the Test Canvas API to
    ///     test against. To setup the Canvas Test, you will need to setup the User Secrets on your
    ///     workstation.
    ///     
    ///     Navigate to the ~\SyllabusZip\SyllabusZip\syllabuszip.tests source directory using your
    ///     command editor.
    ///     
    ///     
    ///     dotnet user-secrets init dotnet 
    ///     user-secrets set CanvasHost "http://canvas.syllabuszip.com/" 
    ///     dotnet user-secrets set ""
    ///     
    ///     To see what secrets are set for your workstation dotnet user-secrets list.
    /// </summary>
    ///
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows"/>
    public abstract class CanvasTestSetup
    {
        private ILoggerFactory _loggerFactory;
        private IConfigurationRoot _configuration;

        protected ServiceCollection ServiceCollection { get; private set; }

        [SetUp]
        public void Setup()
        {
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            //read in configurations 
            _configuration =new ConfigurationBuilder()
                .AddUserSecrets<CanvasTestSetup>()
                .AddEnvironmentVariables()
                .Build();

            // SetUp DI
            ServiceCollection = new ServiceCollection();

            // Set up configurations
            ServiceCollection.AddOptions();
            ServiceCollection.AddScoped((services) => _loggerFactory.CreateLogger<CanvasService>());
            ServiceCollection.Configure<CanvasOptions>((config) =>
            {
                //see the note in the summary about secrets
                config.ApiToken = _configuration["TestCanvasApiToken"];
                config.CanvasHost = _configuration["CanvasHost"];
            });

            ServiceCollection.AddScoped<ICanvasService, CanvasService>();

            
        }
    }}
