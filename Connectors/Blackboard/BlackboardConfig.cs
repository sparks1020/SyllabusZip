using Newtonsoft.Json;
using SyllabusZip.Common.Data;
using SyllabusZip.Connectors.Blackboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyllabusZip.Connectors.Blackboard
{
    public class BlackboardConfig : LmsConfig
    {
        public BlackboardConfig()
        {

            DisplayName = "Blackboard Learn";
            AuthPath = "/learn/api/public/v1/oauth2/authorizationcode?redirect_uri={{REDIRECTURI}}&response_type=code&client_id={{CLIENTID}}&scope=read%20offline&state={{STATE}}";
            TokenPath = "/learn/api/public/v1/oauth2/token?code={{CODE}}&redirect_uri={{REDIRECTURI}}";
            TokenRequestBody = "grant_type=authorization_code";
            ClientId = "95a302d5-8e98-4ef6-924e-edcf553accf3";
            ClientSecret = "QgNCBbvjDmAMxqYqubN6jPk9uNYsNLre";
            // Turns out, blackboard supports the magical "me" user id. This isn't documented anywhere, though
            CourseListPath = "/learn/api/public/v1/users/me/courses?expand=course";
        }

        public override void WillSendApiRequest(HttpClient client, SyllabusSource source, string url, string method, string body)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                source.AuthToken
            );
        }

        public override IList<CourseItem> ParseCourseList(string jsonString)
        {
            var results = JsonConvert.DeserializeObject<CourseResults>(jsonString);
            return results.Results.Select(o => new CourseItem
            {
                Id = o.CourseId,
                Name = o.Course.Name,
                Description = o.Course.Description
            }).ToList();
        }

        public override ICourseImporter CreateCourseImporter(ApplicationDbContext db, HttpClient client, SyllabusSource source)
        {
            return new BlackboardCourseImporter(db, client, source);
        }
    }
}
