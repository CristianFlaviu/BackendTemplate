using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BackendTemplate.Application.ServicesInterface;
using Microsoft.Extensions.Configuration;

namespace BackendTemplate.Infrastructure.Services
{
    public class KeyVaultService: IKeyVaultService
    {
        private readonly SecretClient _secretClient;

        public KeyVaultService(IConfiguration configuration)
        {
            var keyVaultUrl = configuration["AzureKeyVault:VaultUri"];

            if (string.IsNullOrEmpty(keyVaultUrl))
            {
                throw new ArgumentNullException(nameof(keyVaultUrl), "Azure Key Vault URL is missing from configuration.");
            }

            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            var secret = await _secretClient.GetSecretAsync(secretName);
            return secret.Value.Value;
        }
    }
}
