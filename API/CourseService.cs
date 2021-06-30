using Newtonsoft.Json;
using SyllabusZip.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SyllabusZip.API
{
    public class CourseService : IRestService<Course>, IDisposable
    {

        HttpClient client;
        private string access_token;

        public CourseService(Token token)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            access_token = token.access_token;
        }

        public Task<Course> CreateObject(Course T)
        {
            throw new NotImplementedException();
        }

        public Task<Course> DeleteObject()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<Course> ReadObject()
        {
            Course course = new Course();

            var uri = new Uri(Constants.HOSTNAME + Constants.COURSE_PATH + "/externalId:" + Constants.COURSE_ID);

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    course = JsonConvert.DeserializeObject<Course>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {0}", ex.Message);
            }

            return course;
        }

        public Course UpdateObject(Course updateCourse)
        {
            throw new NotImplementedException();
        }

        Task<Course> IRestService<Course>.UpdateObject(Course T)
        {
            throw new NotImplementedException();
        }
    }
}
