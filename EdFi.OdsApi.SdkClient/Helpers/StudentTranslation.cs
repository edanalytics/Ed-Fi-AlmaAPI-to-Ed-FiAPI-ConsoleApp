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
                checkCache = (StudentsResponse)cache.Get("StudentsTranslationData");
            }

            foreach (Student s in checkCache.response)
            {
                if (s.id == studentId)
                {
                    return s;
                }
            }

            return student;
        }


        public static GradeLevel GetStudentGradeLevel(string studentId, string schoolYearId, string schoolId)
        {
            StudentGradeLevelsResponse checkCache = (StudentGradeLevelsResponse)cache.Get(String.Format($"StudentGradeLevel - {schoolId} - {schoolYearId}"));
            GradeLevel gradeLevel = new GradeLevel();

            if (checkCache == null)
            {
                buildStudentGradeLevelCache(schoolYearId, schoolId);
                checkCache = (StudentGradeLevelsResponse)cache.Get(String.Format($"StudentGradeLevel - {schoolId} - {schoolYearId}"));
            }

            foreach (StudentG g in checkCache.students)
            {
                if (g.id == studentId)
                {
                    GradeLevelsResponse glCache = (GradeLevelsResponse)cache.Get(String.Format($"GradeLevel - {g.gradeLevels[0].school} - {schoolYearId}"));
                    if (glCache == null)
                    {
                        buildGradeLevelCache(g.gradeLevels[0].school, schoolYearId);
                        glCache = (GradeLevelsResponse)cache.Get(String.Format($"GradeLevel - {g.gradeLevels[0].school} - {schoolYearId}"));
                    }
                    foreach (GradeLevel gl in glCache.response)
                    {
                        if (gl.id == g.gradeLevels[0].gradeLevelId)
                        {
                            return gl;
                        }
                    }
                }
            }
            return gradeLevel;
        }


        public static UserRole GetStaffUserRole(int schoolId, string roleId)
        {
            UserRoleResponse checkCache = (UserRoleResponse)cache.Get(String.Format($"UserRole"));
            UserRole userRole = new UserRole();

            if (checkCache == null)
            {
                buildUserRoleCache();
                checkCache = (UserRoleResponse)cache.Get(String.Format($"UserRole"));
            }

            foreach (UserRole r in checkCache.userRoles)
            {
                if (r.id == roleId)
                {
                    return r;
                    //UserRoleResponse urCache = (UserRoleResponse)cache.Get(String.Format($"GradeLevel - {schoolId}"));
                    //if (urCache == null)
                    //{
                    //    buildUserRoleCache(schoolId);
                    //    urCache = (UserRoleResponse)cache.Get(String.Format($"GradeLevel - {schoolId}"));
                    //}
                    //foreach (UserRole ur in urCache.userRoles)
                    //{
                    //    if (ur.id == r.id)
                    //    {
                    //        return ur;
                    //    }
                    //}
                }
            }
            return userRole;
        }


        public static string GetSchoolYear()
        {
            var config = GetConfiguration().Build();
            var settings = config.GetSection("Settings").Get<AppSettings>();

            return settings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter;
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

                cache.Set("StudentsTranslationData", studentsResponse);
            }

        }

        public static void buildGradeLevelCache(string schoolId, string schoolYearId)
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
            var request = new RestRequest($"//v2/{schoolId}/grade-levels?schoolYearId={schoolYearId}", DataFormat.Json);
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {

                GradeLevelsResponse gradeLevelResponse = JsonConvert.DeserializeObject<GradeLevelsResponse>(response.Content);


                cache.Set(String.Format($"GradeLevel - {schoolId} - {schoolYearId}"), gradeLevelResponse);
            }
        }

        public static void buildUserRoleCache()
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
            var request = new RestRequest($"//v2/{districtId}/user-roles", DataFormat.Json);
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                UserRoleResponse userRoleResponse = JsonConvert.DeserializeObject<UserRoleResponse>(response.Content);


                cache.Set(String.Format($"UserRole"), userRoleResponse);
            }
        }


        public static void buildStudentGradeLevelCache(string schoolYearId, string schoolId)
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
            var request = new RestRequest($"/v2/{schoolId}/students/grade-levels?schoolYearId={schoolYearId}", DataFormat.Json);
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();

            var response = client.Get(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {

                StudentGradeLevelsResponse studentGradeLevelResponse = JsonConvert.DeserializeObject<StudentGradeLevelsResponse>(JObject.Parse(response.Content)["response"].ToString());


                cache.Set(String.Format($"StudentGradeLevel - {schoolId} - {schoolYearId}"), studentGradeLevelResponse);
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
