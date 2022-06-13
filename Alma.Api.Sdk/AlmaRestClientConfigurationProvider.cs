using Alma.Api.Sdk.Authenticators;
using EdFi.AlmaToEdFi.Common;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.Utf8Json;

namespace Alma.Api.Sdk.Extractors.Alma
{
    public interface IAlmaRestClientConfigurationProvider
    {
        RestClient GetRestClient();
    }

    public class AlmaRestClientConfigurationProvider : IAlmaRestClientConfigurationProvider
    {
        private readonly IAppSettings _settings;
        private RestClient _client;

        public AlmaRestClientConfigurationProvider(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;

            //Generate client (Set BaseURL)
            _client = new RestClient(_settings.AlmaAPI.Connections.Alma.SourceConnection.Url)
            {
                Authenticator = new DigestAuthenticator(_settings.AlmaAPI.Connections.Alma.SourceConnection.Key,
                                                        _settings.AlmaAPI.Connections.Alma.SourceConnection.Secret)
            };

            //JSON serializer settings (Utf8Json is used this time)
            _client.UseUtf8Json();
        }

        public RestClient GetRestClient()
        {
            return _client;
        }
    }
}
