using Newtonsoft.Json;
using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyllabusZip.Connectors.Schoology
{
    public class SchoologyCourseImporter : ICourseImporter
    {
        private const string COURSEID = "{{COURSEID}}";
        private const string COLUMNID = "{{COLUMNID}}";
        private const string USERID = "{{USERID}}";

        private const string CourseDetailsPath = "/sections/{{COURSEID}}";
        private const string CourseMembershipPath = "/sections/{{COURSEID}}/enrollments?type=admin";
        private const string CourseContentsPath = "/learn/api/public/v1/courses/{{COURSEID}}/contents?recursive=true";
        private const string UserDetailsPath = "/users/{{USERID}}";
        private const string AssignmentsPath = "/sections/{{COURSEID}}/assignments?limit=500";

        private ApplicationDbContext Database { get; }
        private HttpClient Client { get; }
        private LmsConfig Config { get; }
        private SyllabusSource Source { get; }

        public SchoologyCourseImporter(ApplicationDbContext db, HttpClient client, LmsConfig config, SyllabusSource source)
        {
            Database = db;
            Client = client;
            Config = config;
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
            IList<SgyUser> instructors = (await GetInstructors(courseId)).Enrollment;
            SgyUser contact = instructors.FirstOrDefault() != null ? (await GetUser(instructors.First().uid)) : null;

            Database.Contact.Add(new ContactInfo
            {
                Id = Guid.NewGuid(),
                SyllabusId = syllabusId,
                Teacher = string.Join("; ", instructors.Select(i => i.name_display)),
                Email = contact?.primary_email,
                //Phone = contact?.BusinessPhone,
                ClassTitle = course.course_title + " " + course.section_title,
            });
            Database.SaveChanges();

            IList<SgyAssignment> assignments = (await GetAssignments(courseId)).Assignment;
            foreach (var assn in assignments)
            {
                Assignment assignment = new Assignment
                {
                    Id = Guid.NewGuid(),
                    SyllabusId = syllabusId,
                    Date = assn.due,
                    Homework = assn.title
                };
                Database.Assignments.Add(assignment);
                Database.SaveChanges();
            }

            return syllabus;
        }

        public Task<Course> GetCourse(string courseId)
        {
            string apiUrl = Source.BaseUrl + CourseDetailsPath.Replace(COURSEID, courseId);
            return Get<Course>(apiUrl);
        }

        public Task<AssignmentList> GetAssignments(string courseId)
        {
            string apiUrl = Source.BaseUrl + AssignmentsPath.Replace(COURSEID, courseId);
            return Get<AssignmentList>(apiUrl);
        }

        public Task<EnrollmentList> GetInstructors(string courseId)
        {
            string apiUrl = Source.BaseUrl + CourseMembershipPath.Replace(COURSEID, courseId);
            return Get<EnrollmentList>(apiUrl);
        }

        public Task<SgyUser> GetUser(string userId)
        {
            string apiUrl = Source.BaseUrl + UserDetailsPath.Replace(USERID, userId);
            return Get<SgyUser>(apiUrl);
        }

        public async Task<T> Get<T>(string url) where T : new()
        {
            Config.WillSendApiRequest(Client, Source, url, "GET", "[]");
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


        public class EnrollmentList
        {
            public IList<SgyUser> Enrollment { get; set; }
        }

        public class SgyUser
        {
            public string uid { get; set; }
            public string name_display { get; set; }
            public string primary_email { get; set; }
        }

        public class Course
        {
            public string course_title { get; set; }
            public string section_title { get; set; }
        }

        public class AssignmentList
        {
            public IList<SgyAssignment> Assignment { get; set; }
        }

        public class SgyAssignment
        {
            public string title { get; set; }
            public string description { get; set; }
            public string due { get; set; }
        }
    }
}
