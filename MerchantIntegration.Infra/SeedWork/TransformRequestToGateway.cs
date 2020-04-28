using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MerchantIntegration.Infra.SeedWork
{
    public static class TransformRequestToGateway
    {
        public static object TreatObject(object objectGeneric)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            var jsonObject = JsonConvert.SerializeObject(
                objectGeneric,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = contractResolver,
                }
            );

            return jsonObject;
        }
    }
}