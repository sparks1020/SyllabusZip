using Newtonsoft.Json.Linq;
using SyllabusZip.Common.Data;
using SyllabusZip.Connectors.Blackboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SyllabusZip.Connectors.Schoology
{
    public class SchoologyConfig : LmsConfig
    {
        private const string RequestTokenPath = "/oauth/request_token";
        private const string UserDetailsPath = "/users/me";

        private const string ConsumerKey = "REDACTED";
        private const string ConsumerSecret = "REDACTED";

        public SchoologyConfig()
        {
            DisplayName = "Schoology";
            AuthPath = "/oauth/authorize?oauth_callback={{REDIRECTURI}}&oauth_token={{AUTHTOKEN}}";
            TokenPath = "/oauth/access_token";
            ClientId = "";
            UsesCentralApi = true;
            BaseUrl = "https://api.schoology.com/v1";
            CourseListPath = "/users/{{USERID}}/sections";
        }

        public override string SiteUrl => "https://schoology.schoology.com";

        public override void WillSendApiRequest(HttpClient client, SyllabusSource source, string url, string method, string body)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", MakeAuthHeader(url, method, body, source));
        }

        public override async Task WillAuthorize(HttpClient client, SyllabusSource source, Action persist)
        {
            string url = BaseUrl + RequestTokenPath;

            WillSendApiRequest(client, source, url, "GET", "[]");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var obj = HttpUtility.ParseQueryString(await response.Content.ReadAsStringAsync());
                // TODO: The following line should not remain here permanently
                Console.WriteLine(obj);
                source.AuthToken = obj["oauth_token"];
                source.AuthTokenSecret = obj["oauth_token_secret"];
                source.AuthTokenExpires = DateTime.UtcNow.AddSeconds(Int32.Parse(obj["xoauth_token_ttl"]));
                //source.SourceUserId = obj["user_id"].ToString();
                persist();
            }
        }

        public override async Task<bool> FinishAuthorization(HttpClient client, SyllabusSource source, Action persist)
        {
            string apiUrl = source.BaseUrl + TokenPath;
            WillSendApiRequest(client, source, apiUrl, "GET", "[]");
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var obj = HttpUtility.ParseQueryString(await response.Content.ReadAsStringAsync());
                source.AuthToken = obj["oauth_token"];
                source.AuthTokenSecret = obj["oauth_token_secret"];
                source.SourceUserId = await GetUserId(client, source);

                persist();
                return true;
            }
            return false;
        }

        private async Task<string> GetUserId(HttpClient client, SyllabusSource source)
        {
            string apiUrl = source.BaseUrl + UserDetailsPath;
            WillSendApiRequest(client, source, apiUrl, "GET", "[]");
            var response = await client.GetAsync(apiUrl);
            if (response.StatusCode == HttpStatusCode.SeeOther)
            {
                return response.Headers.Location.Segments.Last();
            }
            return "";
        }

        public override IList<CourseItem> ParseCourseList(string jsonString)
        {
            JObject obj = JObject.Parse(jsonString);
            var section = obj["section"];
            return section.Select(o => new CourseItem
            {
                Id = o["id"].ToString(),
                Name = (string)o["course_title"],
                Description = "" //(string)o["description"]
            }).ToList();
        }

        public override ICourseImporter CreateCourseImporter(ApplicationDbContext db, HttpClient client, SyllabusSource source)
        {
            return new SchoologyCourseImporter(db, client, this, source);
        }

        /* PORTED FROM https://github.com/schoology/schoology_php_sdk/blob/master/SchoologyApi.class.php */
        private string MakeAuthHeader(string url, string method, string body, SyllabusSource source)
        {
            var timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            var nonce = Guid.NewGuid().ToString("D");

            var oauth_config = new Dictionary<string, string>
            {
                ["oauth_consumer_key"] = ConsumerKey,
                ["oauth_nonce"] = nonce,
                ["oauth_signature_method"] = "HMAC-SHA1",
                ["oauth_timestamp"] = timestamp.ToString(),
                ["oauth_token"] = source.AuthToken,
                ["oauth_version"] = "1.0",
            };
            /* We don't do two-legged OAuth
            if ($this->_is_two_legged){
                $oauth_config['oauth_signature_method'] = 'PLAINTEXT';
            }
            */
            oauth_config["oauth_signature"] = MakeOauthSignature(url, method, ref oauth_config, source);

            return $"realm=\"\", " + String.Join(", ", oauth_config.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        }

        private string MakeOauthSignature(string url, string method, ref Dictionary<string, string> oauth_config, SyllabusSource source)
        {
            // PHP has a function called "hash_hmac". Reimplemented here to the extent it's necessary just to keep the flow
            // from the original PHP below
            // With help from https://stackoverflow.com/questions/12804231/c-sharp-equivalent-to-hash-hmac-in-php
            byte[] hash_hmac(string hash, string message, string key, bool asBinary)
            {
                if (hash != "sha1" || !asBinary)
                    throw new InvalidOperationException("Only SHA1 and return as binary are supported");

                var keyBytes = Encoding.UTF8.GetBytes(key);
                using (var hasher = new HMACSHA1(keyBytes))
                {
                    return hasher.ComputeHash(Encoding.UTF8.GetBytes(message));
                }
            }

            var base_string = MakeBaseString(url, method, ref oauth_config);
            var oauth_str = UpperCaseUrlEncode(ConsumerSecret) + '&' + UpperCaseUrlEncode(source.AuthTokenSecret);
            if (oauth_config["oauth_signature_method"] == "PLAINTEXT")
            {
                return oauth_str;
            }
            var signature = UpperCaseUrlEncode(Convert.ToBase64String(hash_hmac("sha1", base_string, oauth_str, true)));

            return signature;
        }

        private string MakeBaseString(string url, string method, ref Dictionary<string, string> oauth_config)
        {
            // $url shouldn't include parameters
            var idx = url.IndexOf('?');
            var base_url = (idx > -1) ? url.Substring(0, idx) : url;

            string base_string = method + '&' + UpperCaseUrlEncode(base_url) + '&';

            // GET parameters need to be ordered properly with the oauth params
            var oauth_queries = new Dictionary<string, string>();
            var parsed = new UriBuilder(url);
            if (!string.IsNullOrEmpty(parsed.Query))
            {
                var queryParams = HttpUtility.ParseQueryString(parsed.Query);
                foreach (var key in queryParams.AllKeys)
                {
                    oauth_queries[key] = key + '=' + queryParams[key];
                }
            }
            foreach (var kvp in oauth_config)
            {
                oauth_queries[kvp.Key] = kvp.Key + '=' + kvp.Value;
            }

            // Need keys ordered alphabetically
            return base_string + UpperCaseUrlEncode(string.Join('&', oauth_queries.OrderBy(p => p.Key).Select(p => p.Value)));
        }

        // Schoology requires uppercase escape sequences for hash to work right
        public static string UpperCaseUrlEncode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }
    }
}
