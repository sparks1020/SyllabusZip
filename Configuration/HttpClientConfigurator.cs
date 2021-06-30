using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SyllabusZip.Services
{
    public static class HttpClientConfigurator
    {
        /// <summary>
        ///     <<![CDATA[WARNING]]>
        ///     According to the documentation this should work, however for the life of me I can not get
        ///     it to function. At some point the older method will no longer work and we will need to
        ///     investigate this method.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="httpClient">   The httpClient to act on. </param>
        /// <param name="canvasDomain"> The canvas domain. </param>
        /// <param name="apiKey">       The API key. </param>
        ///
        /// <returns>   A HttpClient. </returns>
        public static HttpClient ConfigureCanvasApi(this HttpClient httpClient, string canvasDomain, string apiKey)
        {
            
            httpClient.ConfigureCanvasApi(canvasDomain);

            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey), "API key is a required element.");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            return httpClient;
        }

        /// <summary>
        ///     06/5/2021 - The HTTP client returned does not attempt to encapsulate validation, the
        ///     document Bearer patter does not want to authorize.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="httpClient">   The httpClient to act on. </param>
        /// <param name="canvasDomain"> The canvas domain. </param>
        ///
        /// <returns>   A HttpClient. </returns>

        public static HttpClient ConfigureCanvasApi(this HttpClient httpClient, string canvasDomain)
        {
            if (string.IsNullOrWhiteSpace(canvasDomain)) throw new ArgumentNullException(nameof(canvasDomain), "Canvas domain is a required element.");
            httpClient.BaseAddress = new Uri(canvasDomain, UriKind.Absolute);

            return httpClient;
        }

    }
}
