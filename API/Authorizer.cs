using Newtonsoft.Json;
using SyllabusZip.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SyllabusZip.API
{
    public class Authorizer
    {
        HttpClient client;
        Token token { get; set; }

        public async Task<Token> Authorize()
        {

            var authData = string.Format("{0}:{1}", Constants.KEY, Constants.SECRET);
            Console.WriteLine("authData: {0}", authData);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            Console.WriteLine("authHeaderValue: {0}", authHeaderValue);

            client = new HttpClient();

            var endpoint = new Uri(Constants.HOSTNAME + Constants.AUTH_PATH);
            Console.WriteLine("endpoint: {0}", endpoint);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            Console.WriteLine("headers: {0}", client.DefaultRequestHeaders.ToString());
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            Console.WriteLine("postData: {0}", postData.ToString());

            HttpContent body = new FormUrlEncodedContent(postData);

            Console.WriteLine("body: {0}", body.ToString());

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(endpoint, body).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<Token>(content);
                    Console.WriteLine("Authorize() Token: " + token.ToString());
                }
                else
                {
                    Console.WriteLine(response.ToString());
                    response.EnsureSuccessStatusCode();
                }


            }
            catch (AggregateException agex)
            {
                Console.WriteLine(@"				ERROR {0}\n{1}", agex.Message, agex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"				ERROR {0}", ex.Message);
            }

            return token;
        }
    }
}
