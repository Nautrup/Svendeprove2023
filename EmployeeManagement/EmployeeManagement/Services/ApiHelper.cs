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

            Client.BaseAddress = EndPoint;

            //Client.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImV4cCI6MTY4NTUzNDkyOTY2MiwiaXN0IjoxNjg1NTM0OTI3ODYyLCJpc3MiOiJCb25nb3RydW1tZXIiLCJqdGkiOiIzYjAzNzYwYy1kZjE1LTQ0YWMtODgzNS02NTU2MTQ1ZWRiM2YifQ.eyJ0eXAiOiJhY2Nlc3MiLCJ1aWQiOjEsImNpZCI6MSwicmlkIjoxLCJmbG4iOiJIYW5zIFBldGVyIiwiaGR0IjoxMzY3Mzk2NzczLCJmZHQiOm51bGx9.KR9oyoAb1Vsz15TkjFETQcE3TVEMyfMrgBb0Bv-UDAU");
            
            Client.Headers.Add("Accept", "application/json");
            Client.Headers["Content-Type"] = "application/json";

        }
        // eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImV4cCI6MTY4NTUzNDkyOTY2MiwiaXN0IjoxNjg1NTM0OTI3ODYyLCJpc3MiOiJCb25nb3RydW1tZXIiLCJqdGkiOiIzYjAzNzYwYy1kZjE1LTQ0YWMtODgzNS02NTU2MTQ1ZWRiM2YifQ.eyJ0eXAiOiJhY2Nlc3MiLCJ1aWQiOjEsImNpZCI6MSwicmlkIjoxLCJmbG4iOiJIYW5zIFBldGVyIiwiaGR0IjoxMzY3Mzk2NzczLCJmZHQiOm51bGx9.KR9oyoAb1Vsz15TkjFETQcE3TVEMyfMrgBb0Bv-UDAU
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
            string response = ApiHelper.Client.UploadString(ApiHelper.EndPoint = endpoint, jsonData); 
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
            
            byte[] byteResponse = ApiHelper.Client.UploadData(ApiHelper.EndPoint = endpoint, "PUT", Encoding.UTF8.GetBytes(jsonData));
            string response = Encoding.UTF8.GetString(byteResponse);
            return response;
        }

        /// <summary>
        /// Put til at opdatere data
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Delete(string endpoint, string jsonData)
        {
            // Response
            Client.Headers[HttpRequestHeader.ContentType] = "application/json";

            byte[] byteResponse = ApiHelper.Client.UploadData(ApiHelper.EndPoint = endpoint, "DELETE", new byte[0]);
            string response = Encoding.UTF8.GetString(byteResponse);
            return response;
        }

    }
}
