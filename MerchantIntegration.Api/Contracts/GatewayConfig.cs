using System.Security.Policy;

namespace MerchantIntegration.Api.Contracts
{
    public class GatewayConfig
    {
        public string SecretKey { get; set; }
        public string Url { get; set; }
    }
}