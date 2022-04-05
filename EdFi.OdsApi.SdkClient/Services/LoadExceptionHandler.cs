using EdFi.OdsApi.Sdk.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services
{
    public interface ILoadExceptionHandler
    {
        public void HandleException(Exception ex, object resource);
        public void HandleHttpCode(ApiResponse<object> apiResponse);
    }
    public class LoadExceptionHandler : ILoadExceptionHandler
    {
        public void HandleException(Exception ex, object resource)
        {
            Console.WriteLine($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
        }

        public void HandleHttpCode(ApiResponse<object> apiResponse)
        {
            var okCodes = new List<int> { 200, 201, 202, 203, 204, 205, 206 };

            if (!okCodes.Contains(apiResponse.StatusCode))
                Console.WriteLine($"HttpError {apiResponse.StatusCode}: Resource: {JsonConvert.SerializeObject(apiResponse.Data)}");
        }
    }
}
