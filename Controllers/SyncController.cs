using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SyllabusZip.Common.Data;
using SyllabusZip.Connectors;
using SyllabusZip.Connectors.Blackboard;
using SyllabusZip.Connectors.Blackboard.Models;
using SyllabusZip.Connectors.Schoology;

namespace SyllabusZip.Controllers
{
    [Route("/[Controller]")]
    [Authorize]
    public class SyncController : Controller
    {
        private const string CLIENTID = "{{CLIENTID}}";
        private const string STATE = "{{STATE}}";
        private const string REDIRECTURI = "{{REDIRECTURI}}";
        private const string CODE = "{{CODE}}";
        private const string USERID = "{{USERID}}";
        private const string AUTHTOKEN = "{{AUTHTOKEN}}";

        private static readonly IDictionary<string, LmsConfig> SupportedServices = new Dictionary<string, LmsConfig>(StringComparer.OrdinalIgnoreCase)
        {
            ["blackboard"] = new BlackboardConfig(),
            /*["canvas"] = new LmsConfig
            {
                DisplayName = "Canvas LMS",
                AuthPath = "/login/oauth2/auth?client_id={{CLIENTID}}&response_type=code&state={{STATE}}&redirect_uri={{REDIRECTURI}}",
                ClientId = ""
            },*/
            //["moodle"] = null,
            ["schoology"] = new SchoologyConfig()
        };

        private static readonly HttpClient client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (m, c, x, e) => true,
            AllowAutoRedirect = false
        });

        private ApplicationDbContext Database { get; }

        public SyncController(ApplicationDbContext db)
        {
            Database = db;
        }


        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult ConnectToBlackboard()
        //{
        //    return View();
        //}

        //public async Task<IActionResult> ConnectToBlackboard(string UrlText)
        //{

        //}

        [HttpGet("{service}")]
        public Task<IActionResult> Step1(string service)
        {
            return WithSupportedService(service, async (config) =>
            {
                if (config.UsesCentralApi)
                {
                    return await SaveAndRedirectToSource(config.BaseUrl, service, config.DisplayName);
                }
                return View(config);
            });
        }

        //Canvas documentation shows url as http://www.example.com/oauth2response?code=XXX&state=YYY
        //GET https://<canvas-install-url>/login/oauth2/auth?client_id=XXX&response_type=code&state=YYY&redirect_uri=https://example.com/oauth_complete
        [HttpPost("{service}")]
        public Task<IActionResult> Step1(string service, string UrlText)
        {
            return WithSupportedService(service, async (config) =>
            {
                Uri baseUri = null;
                try
                {
                    baseUri = new Uri(UrlText);
                }
                catch (UriFormatException)
                {
                    return View();
                }

                string baseUrl = baseUri.Scheme + "://" + baseUri.Authority;

                return await SaveAndRedirectToSource(baseUrl, service);
            });
        }

        [HttpGet("{service}/{sourceId}/finish")]
        public Task<IActionResult> Finish(string service, Guid sourceId, string code)
        {
            return WithSupportedService(service, (config) => WithSource(sourceId, async (source) =>
            {
                // TODO: Put this in a blackboardservice
                if (service == "blackboard")
                {
                    string apiUrl = source.BaseUrl + config.TokenPath.Replace(REDIRECTURI, Request.Scheme + "://" + Request.Host + Request.Path).Replace(CODE, code);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.ClientId}:{config.ClientSecret}"))
                    );
                    var response = await client.PostAsync(apiUrl, new StringContent(config.TokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    if (response.IsSuccessStatusCode)
                    {
                        var obj = JObject.Parse(await response.Content.ReadAsStringAsync());
                        // TODO: The following line should not remain here permanently
                        Console.WriteLine(obj);
                        source.AuthToken = obj["access_token"].ToString();
                        source.AuthTokenExpires = DateTime.UtcNow.AddSeconds(obj["expires_in"].Value<int>());
                        source.SourceUserId = obj["user_id"].ToString();
                        Database.SaveChanges();

                        return Redirect($"/sync/{service}/{sourceId}/choose");
                    }
                }
                else
                {
                    if (await config.FinishAuthorization(client, source, () => Database.SaveChanges()))
                        return Redirect($"/sync/{service}/{sourceId}/choose");
                }

                // TODO: Create this view
                return Redirect($"/sync/{service}/{sourceId}/failed");
            }));
        }

        [HttpGet("{service}/{sourceId}/choose")]
        public Task<IActionResult> Choose(string service, Guid sourceId)
        {
            return WithSupportedService(service, (config) => WithAuthenticatedSource(sourceId, async (source) =>
            {
                string apiUrl = source.BaseUrl + config.CourseListPath.Replace(USERID, source.SourceUserId);
                config.WillSendApiRequest(client, source, apiUrl, "GET", "[]");
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return View(config.ParseCourseList(await response.Content.ReadAsStringAsync()));
                }

                Console.WriteLine(await response.Content.ReadAsStringAsync());

                return Redirect($"/sync/{service}/{sourceId}/failed");
            }));
        }

        [HttpPost("{service}/{sourceId}/choose")]
        public Task<IActionResult> Choose(string service, Guid sourceId, string[] courseIds)
        {
            return WithSupportedService(service, (config) => WithAuthenticatedSource(sourceId, async (source) =>
            {
                var importer = config.CreateCourseImporter(Database, client, source);

                foreach (string courseId in courseIds)
                {
                    Syllabus syllabus = await importer.ImportCourse(courseId);
                    syllabus.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Database.SaveChanges();
                }

                return Redirect("/");
            }));
        }


        private Task<IActionResult> WithSupportedService(string service, Func<LmsConfig, Task<IActionResult>> func)
        {
            if (!SupportedServices.ContainsKey(service))
                return Task.FromResult<IActionResult>(NotFound());

            return func(SupportedServices[service]);
        }

        private Task<IActionResult> WithSource(Guid sourceId, Func<SyllabusSource, Task<IActionResult>> func)
        {
            SyllabusSource source = Database.SyllabusSources.FirstOrDefault(s => s.Id == sourceId);
            if (source is null)
                return Task.FromResult<IActionResult>(NotFound());

            return func(source);
        }

        private Task<IActionResult> WithAuthenticatedSource(Guid sourceId, Func<SyllabusSource, Task<IActionResult>> func)
        {
            return WithSource(sourceId, (source) =>
            {
                if (string.IsNullOrEmpty(source.AuthToken) || source.AuthTokenExpires < DateTime.UtcNow.AddSeconds(-10))
                {
                    // TODO: Try refresh token

                    return Task.FromResult<IActionResult>(Redirect(MakeAuthUrl(source.BaseUrl, source.Service, source, SupportedServices[source.Service])));
                }

                return func(source);
            });
        }

        private string MakeAuthUrl(string baseUrl, string service, SyllabusSource source, LmsConfig config)
        {
            return baseUrl + config.AuthPath
                .Replace(CLIENTID, config.ClientId)
                .Replace(STATE, "")
                .Replace(REDIRECTURI, HttpUtility.UrlEncode(Request.Scheme + "://" + Request.Host + $"/sync/{service}/{source.Id}/finish"))
                .Replace(AUTHTOKEN, source.AuthToken);
        }

        private async Task<RedirectResult> SaveAndRedirectToSource(string baseUrl, string service, string displayName = null)
        {
            SyllabusSource source = await SaveSource(baseUrl, service);
            LmsConfig config = SupportedServices[service];

            await config.WillAuthorize(client, source, () => Database.SaveChanges());

            string apiURL = MakeAuthUrl(config.UsesCentralApi ? config.SiteUrl : baseUrl, service, source, config);

            return Redirect(apiURL);
        }

        private async Task<SyllabusSource> SaveSource(string baseUrl, string service, string displayName = null)
        {
            Guid sourceId = Guid.NewGuid();
            SyllabusSource source = new SyllabusSource
            {
                Id = sourceId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Service = service.ToLowerInvariant(),
                BaseUrl = baseUrl,
                DisplayName = displayName ?? baseUrl,
            };
            Database.SyllabusSources.Add(source);
            await Database.SaveChangesAsync();

            return source;
        }
    }
}
