using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyllabusZip.Connectors
{
    public class CourseItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public interface ICourseImporter
    {
        Task<Syllabus> ImportCourse(string courseId);
    }

    public abstract class LmsConfig
    {
        public string DisplayName { get; set; }
        public string AuthPath { get; set; }
        public string TokenPath { get; set; }
        public string TokenRequestBody { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CourseListPath { get; set; }
        public string CourseDetailsPath { get; set; }
        public bool UsesCentralApi { get; set; }
        public string BaseUrl { get; set; }
        public virtual string SiteUrl => BaseUrl;
        public virtual void WillSendApiRequest(HttpClient client, SyllabusSource source, string url, string method, string body) { }
        public virtual Task WillAuthorize(HttpClient client, SyllabusSource source, Action persist) { return Task.CompletedTask; }
        public virtual Task<bool> FinishAuthorization(HttpClient client, SyllabusSource source, Action persist) { return Task.FromResult(true); }
        public abstract IList<CourseItem> ParseCourseList(string jsonString);
        public abstract ICourseImporter CreateCourseImporter(ApplicationDbContext db, HttpClient client, SyllabusSource source);
    }
}
