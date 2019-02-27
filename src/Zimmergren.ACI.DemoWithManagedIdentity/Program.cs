using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json.Linq;

namespace Zimmergren.ACI.DemoWithManagedIdentity
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to another ACI Demo!");
            while (true)
            {
                Console.WriteLine($"{Environment.NewLine}START {DateTime.UtcNow} ({Environment.MachineName})");

                ProcessRequest();
                
                Thread.Sleep(2500);
            }
        }

        private static void ProcessRequest()
        {
            var secretName = "TobiSecretOne";
            var secretValue = GetSecretFromKeyVault(secretName);

            Console.WriteLine($" Secret '{secretName}' has value '{secretValue}'");
        }

        /// <summary>
        /// Gets a given secret from the Key Vault
        /// </summary>
        /// <param name="secretName">Name of the secret</param>
        /// <returns>String value of the secret</returns>
        private static string GetSecretFromKeyVault(string secretName)
        {
            var keyVault = new KeyVaultClient(GetAccessTokenAsync);
            var secretResult = keyVault.GetSecretAsync($"https://myacidemovault.vault.azure.net", secretName).Result;

            return secretResult.Value;
        }

        /// <summary>
        /// Gets an Access Token (Authorization Bearer token) for the given resource.
        /// Using the public Azure Instance Metadata Service endpoint, which is only accessible from inside this container,
        /// and only if your service principal that is now attached to your
        /// ACI container group already has been granted permissions to the resource you're targeting.
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource">Resource URI of the targeted service ("https://vault.azure.com", for example)</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            // If the token expires within the next five minutes, we'll grab a new one.
            if (_cachedToken != null)
            {
                if (_cachedToken.ExpiresOn > DateTime.UtcNow.AddSeconds(30))
                {
                    // Use the existing token, it's still valid.
                    return _cachedToken.AccessToken;
                }
            }

            var aimsEndpoint = "169.254.169.254";
            var apiVersion = "2018-02-01";

            var aimsUri = $"http://{aimsEndpoint}/metadata/identity/oauth2/token?api-version={apiVersion}&resource={HttpUtility.UrlEncode(resource)}";

            HttpClient client = new HttpClient();
            var response = client.GetStringAsync(aimsUri).Result;

            // Parse the Json response and pick the "access_token" property, which has the value of our Bearer authorization token.
            var rawResponse = JObject.Parse(response);
            var accessTokenValue = rawResponse["access_token"].Value<string>();
            var expiresOnValue = rawResponse["expires_on"].Value<int>();

            // There's frameworks and helpers for this, but for clarity in this example
            // I think it makes sense to explain exactly how this is happening, which should
            // be clear from the below code sample. (expires_on in a jwt token is in seconds since Unix epoch).
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expiryDate = epoch.AddSeconds(expiresOnValue);
            _cachedToken = new CachedAccessToken(accessTokenValue, expiryDate);

            return accessTokenValue;
        }

        private static CachedAccessToken _cachedToken;
    }

    public class CachedAccessToken
    {
        public string AccessToken { get; }
        public DateTime ExpiresOn { get; }

        public CachedAccessToken(string accessToken, DateTime expiresOn)
        {
            AccessToken = accessToken;
            ExpiresOn = expiresOn;
        }
    }
}
