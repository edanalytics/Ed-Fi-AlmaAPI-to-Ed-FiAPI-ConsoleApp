using System.Net;
using System.Runtime.Serialization;
using System.Security.Authentication;
using EdFi.OdsApi.Sdk.Client;
using RestSharp.Portable;

namespace EdFi.AlmaToEdFi.Cmd.Services
{
    class TokenRetriever
    {
        private string oauthUrl;
        private string clientKey;
        private string clientSecret;

        public TokenRetriever(string oauthUrl, string clientKey, string clientSecret)
        {
            this.oauthUrl = oauthUrl;
            this.clientKey = clientKey;
            this.clientSecret = clientSecret;
        }

        public string ObtainNewBearerToken()
        {
            var oauthClient = new ApiClient(oauthUrl);
            return GetBearerToken(oauthClient);
        }

        private string GetBearerToken(ApiClient oauthClient)
        {
            var bearerTokenRequest = new RestRequest("oauth/token", Method.POST);
            bearerTokenRequest.AddParameter("Client_id", clientKey);
            bearerTokenRequest.AddParameter("Client_secret", clientSecret);
            bearerTokenRequest.AddParameter("Grant_type", "client_credentials");

            var bearerTokenResponse = oauthClient.RestClient.Execute<BearerTokenResponse>(bearerTokenRequest).Result;
            if (bearerTokenResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new AuthenticationException("Unable to retrieve an access token. Error message: " +
                                                  bearerTokenResponse.Data.Error);
            }

            if (bearerTokenResponse.Data.Error != null || bearerTokenResponse.Data.TokenType != "bearer")
            {
                throw new AuthenticationException(
                    "Unable to retrieve an access token. Please verify that your application secret is correct.");
            }

            return bearerTokenResponse.Data.AccessToken;
        }
    }

    [DataContract]
    internal class BearerTokenResponse
    {
        [DataMember(Name = "access_token", EmitDefaultValue = false)]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in", EmitDefaultValue = false)]
        public string ExpiresIn { get; set; }

        [DataMember(Name = "token_type", EmitDefaultValue = false)]
        public string TokenType { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }
    }
}