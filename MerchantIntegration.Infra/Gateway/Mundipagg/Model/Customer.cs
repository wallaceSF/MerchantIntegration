namespace MerchantIntegration.Infra.Gateway.Mundipagg.Model
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Document { get; set; }
        public Phones Phones { get; set; }
        public Address Address { get; set; }
        public Metadata Metadata { get; set; }
    }
}