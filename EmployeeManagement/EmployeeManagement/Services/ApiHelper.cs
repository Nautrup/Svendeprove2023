using EmployeeManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace EmployeeManagement.Services
{
    public static class ApiHelper
    {
        public static string EndPoint = "http://localhost:6969";

        public static WebClient Client { get; set; }
        public static string Token { get; set; }


        public static void InitializeClient()
        {
            Client = new WebClient();

            if (Client.BaseAddress != "http://localhost:6969")
            {
                EndPoint = "http://localhost:6969";
            }

            Client.BaseAddress = EndPoint;

            
            Client.Headers.Add("Accept", "application/json");
            Client.Headers["Content-Type"] = "application/json";

        }
       
        public static void SetToken(string token)
        {
            Client.Headers.Add("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Post til at sende data
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Post(string endpoint, string jsonData)
        {
            // Response
            Client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string response = ApiHelper.Client.UploadString(ApiHelper.EndPoint = endpoint, "POST", jsonData); 
            return response;
        }

        /// <summary>
        /// Get til at hente data
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Get(string endpoint, Dictionary<string, string>? parameters = null)
        {
            var builder = new StringBuilder();
            builder.Append(endpoint);
            Client.Headers[HttpRequestHeader.ContentType] = "application/json";
            if (parameters != null && parameters.Count > 0)
            {
                builder.Append("?");
                foreach (var parameter in parameters)
                {
                    if (builder.Length != 0)
                    {
                        builder.Append("&");
                    }

                    builder.Append($"{parameter.Key}={HttpUtility.UrlEncode(parameter.Value)}");
                }
            }
            Debug.Write(builder.ToString());
            return Client.DownloadString(builder.ToString());
        }

        /// <summary>
        /// Put til at opdatere data
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Put(string endpoint, string jsonData)
        {
            // Response
            Client.Headers[HttpRequestHeader.ContentType] = "application/json";

            string response = Client.UploadString(ApiHelper.EndPoint = endpoint, "PUT", jsonData);
            
            return response;
        }

        /// <summary>
        /// Til at sletter brugeren
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Delete(string endpoint)
        {
            // Response
            Client.Headers[HttpRequestHeader.ContentType] = "application/json";

            string response = ApiHelper.Client.UploadString(ApiHelper.EndPoint = endpoint, "DELETE", string.Empty);
            
            return response;
        }

    }
}
