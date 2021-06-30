using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace SyllabusZip.Services
{
    /// <summary>
    ///     An implementation of a service that fullfiles <see cref="ICanvasService"/> using the API
    ///     Documenation found at  API Doc - https://canvas.instructure.com/doc/api/courses.html.
    /// </summary>
    public class CanvasService : ICanvasService
    {
        private CanvasOptions _configuration;
        private ILogger<CanvasService> _logger;

        private HttpClient _client { get; }
        public IServiceProvider ServiceProvider { get; }

        /// <summary>   Constructor - builds Http Client based on Domain and Key. </summary>
        ///
        /// <param name="options">  The domain address to the Canvas Instance. </param>
        /// <param name="logger">   The API Key defined on the Canvas Instance for this application. </param>
        public CanvasService(IOptions<CanvasOptions> options, ILogger<CanvasService> logger)
        {
            _configuration = options.Value;
            _logger = logger;
            _client = new HttpClient().ConfigureCanvasApi(_configuration.CanvasHost);

        }

        /// <summary>   Attempts to get courses. </summary>
        ///
        /// <param name="courses">  [out] The courses. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        public bool TryGetCourses(out dynamic courses)
        {
            courses = string.Empty;
            // Returns an array of courses as JSON.
            try
            {
                using var response = _client.GetAsync("api/v1/courses?access_token=" + _configuration.ApiToken).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Content == null)
                    {
                        _logger.LogDebug("Response content was null");
                        return false;
                    }

                    try
                    {
                        using var streamReader = new StreamReader(response.Content.ReadAsStreamAsync().Result);
                        using var jsonReader = new JsonTextReader(streamReader);
                        var serializer = new JsonSerializer();
                        courses = serializer.Deserialize(jsonReader);
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"unable to read or deserialize response content");
                        return false;
                    }
                }
                _logger.LogError($"Did not receive 200 OK status code: {response.StatusCode}");
                return false;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception when calling the service");
                // Here it's up to you if you want to throw or return Retry/Fail, im choosing to FAIL.
                return false;
            }
        }
    }
}
