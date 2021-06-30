using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SyllabusZip.Common.Data;
using SyllabusZip.DevServices;
using SyllabusZip.Models;
using SyllabusZip.Services;

namespace SyllabusZip.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        private const string bucketName = "syllabusstorage";
        private const string devbucketname = "syllabusstorage-dev";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;
        private static AmazonS3Client client;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, ApplicationDbContext applicationDbContext, IEmailSender emailSender)
        {
            _logger = logger;
            _environment = hostingEnvironment;
            _context = applicationDbContext;
            _emailSender = emailSender;

        }

        [AllowAnonymous]
        public IActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                return RedirectToAction("AuthenticatedIndex", "Home");
            }
            return View("Homepage_LoggedOut");
        }

        public IActionResult AuthenticatedIndex()
        {
            return WithPlanSelected(() =>
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                AuthenticatedIndexViewModel authmodel = new AuthenticatedIndexViewModel();
                authmodel.Assignment = new DateTimeConvert(_context).CalculateAssignmentDate(userId);
                authmodel.Exam = new DateTimeConvert(_context).CalculateExamDate(userId);
                return View("AuthenticatedIndex", authmodel);
            });
        }

        [AllowAnonymous]
        public IActionResult Homepage()
        {
            return View("Homepage_LoggedOut");
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Course(Guid syllabusId)
        {
            return WithPlanSelected(() =>
            {
                //add section of updating and reading from DB
                ContactInfo course = _context.Contact.Where(course => course.SyllabusId == syllabusId)
                .FirstOrDefault();

                if (course == null)
                {
                    return NotFound();
                }
                return View(course);
            });
        }

        public class CalendarViewModel
        {
            public Assignment Assignment { get; set; }
            public Exam Exam { get; set; }
        }


        public IActionResult Calendar(Guid syllabusId)
        {

            return WithPlanSelected(() => View());

        }

        public IActionResult Exams(Guid syllabusId)
        {
            return WithPlanSelected(() =>
            {
                //add section of updating and reading from DB
                IList<Exam> exam = _context.Exams.Where(exam => exam.SyllabusId == syllabusId)
                .ToList();

                return View(exam);
            });
        }

        public IActionResult Assignments(Guid syllabusId)
        {
            return WithPlanSelected(() =>
            {
                //add section of updating and reading from DB
                IList<Assignment> assignment = _context.Assignments.Where(assignment => assignment.SyllabusId == syllabusId)
                .ToList();
                return View(assignment);
            });
        }

        public IActionResult Materials(Guid syllabusId)
        {
            return WithPlanSelected(() =>
            {
                //add section of updating and reading from DB
                IList<Materials> materials = _context.Materials.Where(materials => materials.SyllabusId == syllabusId)
                .ToList();
                return View(materials);
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ContactInfo(Guid syllabusId)
        {
            return WithPlanSelected(() =>
            {
                IList<ContactInfo> contactinfo = _context.Contact.Where(contactinfo => contactinfo.SyllabusId == syllabusId)
                    .ToList();
                return View(contactinfo);
            });
        }

        [AllowAnonymous]
        public IActionResult AboutTheTechnology() => View();

        [AllowAnonymous]
        public IActionResult Students() => View();

        public IActionResult GetCalendarData([FromServices] CalendarService calendarService)
        {
            return Json(calendarService.GetEvents(e => e.Syllabus.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && e.Syllabus.CourseStatus, addToday: true));
        }

        [AllowAnonymous]
        public IActionResult Pricing() => View();

        [AllowAnonymous]
        public IActionResult FAQ() => View();

        [AllowAnonymous]
        public IActionResult Error_404() => View();

        public IActionResult Contact() => View();

        [HttpPost]
        public async Task<IActionResult> ContactAsync(string name, string email, string subject, string message)
        {
            string emailtitle = "Contact Form";
            string emailbody = message;
            string emailcontents = emailtitle + "<br/>" + emailbody + "<br/>" + email;

            await _emailSender.SendEmailAsync("support@syllabuszip.com", subject, emailcontents);

            return RedirectToAction(nameof(ContactSuccess));
        }

        [HttpGet]
        public IActionResult ContactSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadSyllabus()
        {
            return WithPlanSelected(() =>
            {
                var sources = _context.SyllabusSources.Where(s => s.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
                return View(sources);
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadSyllabusAsync(IFormFile FileUpload_FormFile, DateTime CourseFirstDay, [FromServices] IConfiguration configuration, [FromServices] IServiceProvider services)
        {

            client = new AmazonS3Client(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"), Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY"), bucketRegion);

            //Assigning Unique Filename (Guid)
            var syllabusId = Guid.NewGuid();
            var myUniqueFileName = Convert.ToString(syllabusId);

            //Getting file Extension
            var FileExtension = Path.GetExtension(FileUpload_FormFile.FileName);

            // concatenating FileName + FileExtension
            var newFileName = myUniqueFileName + FileExtension;

            try
            {
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = configuration.GetValue<bool>("UseDevServices") ? devbucketname : bucketName,
                    Key = newFileName,
                    InputStream = FileUpload_FormFile.OpenReadStream(),
                    ContentType = FileUpload_FormFile.ContentType
                };
                PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered ***. Message: '{0}' when writing object", e);
            }
            catch (Exception e)
            {
                _logger.LogError("Unencountered on server. Message: '{0}' when writing an object", e);
            }

            Syllabus syllabus = new Syllabus
            {
                Id = syllabusId,
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value,
                CourseStatus = true,
                CourseFirstDay = CourseFirstDay
            };
            _context.Syllabi.Add(syllabus);
            _context.SaveChanges();

            if (configuration.GetValue<bool>("UseDevServices"))
            {
                var analyzer = services.GetService(typeof(IngestAnalyzeStoreService)) as IngestAnalyzeStoreService;
#pragma warning disable 4014
                // Async task is not awaited and may continue to run after the function has finished
                analyzer.Run(syllabus, devbucketname, newFileName, FileUpload_FormFile.FileName)
                    .ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
            }

            //// Check whether the connection string can be parsed.
            //CloudStorageAccount storageAccount;
            //if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            //{
            //    // If the connection string is valid, proceed with operations against Blob
            //    // storage here.
            //    // Create the CloudBlobClient that represents the
            //    // Blob storage endpoint for the storage account.
            //    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            //    CloudBlobContainer cloudBlobContainer =
            //    cloudBlobClient.GetContainerReference("syllabus");


            //    // Get a reference to the blob address, then upload the file to the blob.
            //    // Use the value of localFileName for the blob name.
            //    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(newFileName);
            //    await cloudBlockBlob.UploadFromStreamAsync(FileUpload_FormFile.OpenReadStream());

            //    _context.Syllabi.Add(new Syllabus
            //    {
            //        Id = syllabusId,
            //        UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
            //    });
            //    _context.SaveChanges();
            //}
            //else
            //{
            //    // Otherwise, let the user know that they need to define the environment variable.
            //    Console.WriteLine(
            //        "A connection string has not been defined in the system environment variables. " +
            //        "Add an environment variable named 'AZURE_STORAGE_CONNECTION_STRING' with your storage " +
            //        "connection string as a value.");
            //    Console.WriteLine("Press any key to exit the application.");
            //    Console.ReadLine();
            //}

            return View("UploadSuccess");
        }

        private IActionResult WithPlanSelected(Func<IActionResult> action, Func<IActionResult> fallbackAction = null)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (string.IsNullOrWhiteSpace(user?.CustomerId))
                return fallbackAction?.Invoke() ?? Redirect("/Join");

            // Aside from manually, there's no way to set an expiration in development, so it's always MinValue
            if (user.Expiration != DateTime.MinValue && user.Expiration < DateTime.UtcNow.AddDays(-1))
                return fallbackAction?.Invoke() ?? Redirect("/Join");

            return action();
        }
    }
}
