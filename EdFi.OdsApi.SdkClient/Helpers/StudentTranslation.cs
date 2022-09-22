using Alma.Api.Sdk.Authenticators;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
    public class StudentTranslation
    {

        static IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

        public static Student GetStudentById(string studentId)
        {

            var config = GetConfiguration().Build();
            var settings = config.GetSection("Settings").Get<AppSettings>();

            StudentsResponse checkCache = (StudentsResponse)cache.Get("StudentsTranslationData");
            Student student = new Student();

            if (checkCache == null)
            {
                buildCache();
            }
            else
            {
                foreach (Student s in checkCache.response)
                {
                    if (s.id == studentId)
                    {
                        return s;
                    }
                }
            }

            var client = new RestClient(settings.AlmaAPI.Connections.Alma.SourceConnection.Url)
            {
                Authenticator = new DigestAuthenticator(settings.AlmaAPI.Connections.Alma.SourceConnection.Key,
                                                        settings.AlmaAPI.Connections.Alma.SourceConnection.Secret)
            };

            //JSON serializer settings (Utf8Json is used this time)
            client.UseUtf8Json();
            string districtId = settings.AlmaAPI.Connections.Alma.SourceConnection.District;
            var request = new RestRequest($"/v2/{districtId}/students/{studentId}", DataFormat.Json);
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            var response = client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new Student();


            //var studentResponse = new Utf8JsonSerializer().Deserialize<StudentsResponse>(response);
            StudentsResponse studentsResponse = JsonConvert.DeserializeObject<StudentsResponse>(WrapInJsonList(response.Content));
            student = studentsResponse.response[0];

            return student;
        }

        public static void buildCache()
        {
            var config = GetConfiguration().Build();
            var settings = config.GetSection("Settings").Get<AppSettings>();

            var client = new RestClient(settings.AlmaAPI.Connections.Alma.SourceConnection.Url)
            {
                Authenticator = new DigestAuthenticator(settings.AlmaAPI.Connections.Alma.SourceConnection.Key,
                                                        settings.AlmaAPI.Connections.Alma.SourceConnection.Secret)
            };

            //JSON serializer settings (Utf8Json is used this time)
            client.UseUtf8Json();
            string districtId = settings.AlmaAPI.Connections.Alma.SourceConnection.District;
            var request = new RestRequest($"/v2/{districtId}/students", DataFormat.Json);
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StudentsResponse studentsResponse = JsonConvert.DeserializeObject<StudentsResponse>(response.Content);
                //Dictionary<string, string> idMapping = new Dictionary<string, string>();
                //foreach (Student s in studentsResponse.response)
                //{
                //    if (s.stateId != null)
                //    {
                //        idMapping.Add(s.id, s.stateId);
                //    }
                //    else
                //    {
                //        idMapping.Add(s.id, s.id);
                //    }
                //    cache.Set("idMapping", idMapping);

                //}

                cache.Set("StudentsTranslationData", studentsResponse);
            }
            
        }


        private static IConfigurationBuilder GetConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            // Switch Config Provider based on appSettings.json
            var settings = configBuilder.Build().GetSection("Settings").Get<AppSettings>();
            if (settings.AlmaAPI.ParameterStoreProvider.ToLower().Contains("awsparamstore"))
            {
                //configBuilder.AddSystemsManager("/AlmaApi/",
                //                                new AWSOptions
                //                                {
                //                                    Region = RegionEndpoint.GetBySystemName(awsRegion)
                //                                }, TimeSpan.FromSeconds(20));
                configBuilder.AddSystemsManager("/AlmaApi/", TimeSpan.FromSeconds(20));
            }
            return configBuilder;
        }

        private static string WrapInJsonList(string json)
        {
            //Hacky way of dealing with the endpoint returning one student instead of a Json list of students.
            JArray jsonArray = new JArray();
            JObject s = JObject.Parse(json);
            JObject o = new JObject();
            jsonArray.Add(s["response"]);
            o["response"] = jsonArray;

            return o.ToString();
        }
    }
}
