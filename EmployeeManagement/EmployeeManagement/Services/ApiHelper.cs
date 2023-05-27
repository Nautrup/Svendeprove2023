using EmployeeManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static void InitializeClient()
        {
            Client = new WebClient();

            Client.BaseAddress = EndPoint;

            Client.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiIsImV4cCI6MTY4NTAzNzgyNzc4NCwiaXN0IjoxNjg1MDM3ODI1OTg0LCJpc3MiOiJCb25nb3RydW1tZXIiLCJqdGkiOiJmZjI3NDU5Zi1hMzIxLTQwYjMtOGFiZS00NWY3MGE1MWMwZDMifQ.eyJ0eXAiOiJhY2Nlc3MiLCJ1aWQiOjEsImNpZCI6MX0.u-iyd89k__gNei_03sZsHxJq4r7aXFgHDEfU3abdP7o");
            
            Client.Headers.Add("Accept", "application/json");
            Client.Headers["Content-Type"] = "application/json";

        }

        /// <summary>
        /// Retunere json data
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="jsonData">Json Data</param>
        /// <returns></returns>
        public static string Post(string endpoint, string jsonData)
        {
            // Response
            return ApiHelper.Client.UploadString(ApiHelper.EndPoint = endpoint, jsonData);
        }

        public static string Get(string endpoint, Dictionary<string, string>? parameters = null)
        {
            var builder = new StringBuilder();
            builder.Append(endpoint);

            if (parameters != null && parameters.Count > 0)
            {
                builder.Append("?");
                foreach (var parameter in parameters)
                {
                    builder.Append($"{parameter.Key}={HttpUtility.UrlEncode(parameter.Value)}");
                }
            }

            return Client.DownloadString(builder.ToString());
        }

    }
}
