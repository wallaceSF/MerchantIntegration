using MerchantIntegration.Api.Contracts;

namespace MerchantIntegration.Api.Model
{
    public class ConnectionMongoSettings : IConnectionMongoSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}