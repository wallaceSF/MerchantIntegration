namespace MerchantIntegration.Api.Contracts
{
    public interface IConnectionMongoSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}