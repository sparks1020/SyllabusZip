using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyllabusZip.Common.Data;
using SyllabusZip.Connectors.Blackboard.Models;

namespace SyllabusZip.Connectors.Blackboard
{
    public class BlackboardCourseImporter : ICourseImporter
    {
        private const string COURSEID = "{{COURSEID}}";
        private const string COLUMNID = "{{COLUMNID}}";
        private const string USERID = "{{USERID}}";

        private const string CourseDetailsPath = "/learn/api/public/v3/courses/{{COURSEID}}";
        private const string CourseMembershipPath = "/learn/api/public/v1/courses/{{COURSEID}}/users";
        private const string CourseContentsPath = "/learn/api/public/v1/courses/{{COURSEID}}/contents?recursive=true";
        private const string GradebookColumnPath = "/learn/api/public/v2/courses/{{COURSEID}}/gradebook/columns/{{COLUMNID}}";
        private const string UserDetailsPath = "/learn/api/public/v1/users/{{USERID}}";

        private ApplicationDbContext Database { get; }
        private HttpClient Client { get; }
        private SyllabusSource Source { get; }

        public BlackboardCourseImporter(ApplicationDbContext db, HttpClient client, SyllabusSource source)
        {
            Database = db;
            Client = client;
            Source = source;
        }

        public async Task<Syllabus> ImportCourse(string courseId)
        {
            Guid syllabusId = Guid.NewGuid();
            Syllabus syllabus = new Syllabus
            {
                Id = syllabusId,
                SourceId = Source.Id,
                CourseStatus = true,
            };
            Database.Syllabi.Add(syllabus);
            Database.SaveChanges();

            Course course = await GetCourse(courseId);
            IList<Models.User> instructors = (await GetInstructors(courseId)).Results
                .Where(m => m.CourseRoleId == "Instructor")
                .Select(m => m.User)
                .ToList();
            Contact contact = instructors.FirstOrDefault()?.Contact;
            if (contact is null)
            {
                string userId = instructors.FirstOrDefault()?.Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    contact = (await GetUser(userId)).Contact;
                }
            }

            Database.Contact.Add(new ContactInfo
            {
                Id = Guid.NewGuid(),
                SyllabusId = syllabusId,
                Teacher = string.Join("; ", instructors.Select(i => i.Name.DisplayName)),
                Email = contact?.Email,
                Phone = contact?.BusinessPhone,
                ClassTitle = course.Name,
            });
            Database.SaveChanges();

            await ImportCourseDetails(courseId, syllabusId);

            return syllabus;
        }

        public async Task ImportCourseDetails(string courseId, Guid syllabusId)
        {
            string apiUrl = Source.BaseUrl + CourseContentsPath.Replace(COURSEID, courseId);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                Source.AuthToken
            );
            var response = await Client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                var results = JsonConvert.DeserializeObject<CourseContentsResults>(responseBody);

                foreach (var item in results.Results)
                {
                    switch (item.ContentHandler.Id)
                    {
                        case ContentHandler.Syllabus:
                            Console.WriteLine("Handling syllabus");
                            // TODO
                            break;
                        case ContentHandler.Assignment:
                            GradebookColumn column = await GetGradebookColumn(courseId, item.ContentHandler.GradeColumnId);
                            Database.Assignments.Add(new Assignment
                            {
                                Id = Guid.NewGuid(),
                                SyllabusId = syllabusId,
                                Date = column.Grading.Due.ToString(),
                                Homework = item.Title,
                            });
                            Database.SaveChanges();
                            break;
                        case ContentHandler.Folder:
                            // Ignored intentionally
                            break;
                        default:
                            Console.WriteLine($"Content type {item.ContentHandler.Id} is not handled");
                            break;
                    }
                }

                return;
            }

            Console.WriteLine(await response.Content.ReadAsStringAsync());

            // TODO: Throw a custom exception
            throw new Exception();
        }

        public Task<Course> GetCourse(string courseId)
        {
            string apiUrl = Source.BaseUrl + CourseDetailsPath.Replace(COURSEID, courseId);
            return Get<Course>(apiUrl);
        }

        public Task<GradebookColumn> GetGradebookColumn(string courseId, string gradeColumnId)
        {
            string apiUrl = Source.BaseUrl + GradebookColumnPath.Replace(COURSEID, courseId).Replace(COLUMNID, gradeColumnId);
            return Get<GradebookColumn>(apiUrl);
        }

        public Task<ResultsList<CourseMembership>> GetInstructors(string courseId)
        {
            string apiUrl = Source.BaseUrl + CourseMembershipPath.Replace(COURSEID, courseId) + "?role=Instructor&expand=user,contact";
            return Get<ResultsList<CourseMembership>>(apiUrl);
        }

        public Task<User> GetUser(string userId)
        {
            string apiUrl = Source.BaseUrl + UserDetailsPath.Replace(USERID, userId);
            return Get<User>(apiUrl);
        }

        public async Task<T> Get<T>(string url) where T : new()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                Source.AuthToken
            );
            var response = await Client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                return JsonConvert.DeserializeObject<T>(responseBody);
            }

            Console.Error.WriteLine(await response.Content.ReadAsStringAsync());
            return new T();
        }
    }
}