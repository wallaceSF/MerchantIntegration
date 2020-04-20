namespace MerchantIntegration.Infra.Gateway.Mundipagg.Model
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string document { get; set; }
        public Phones phones { get; set; }
        public Address address { get; set; }
        public Metadata metadata { get; set; }
    }
}