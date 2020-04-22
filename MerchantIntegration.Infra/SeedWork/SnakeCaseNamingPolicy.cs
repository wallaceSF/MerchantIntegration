using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace MerchantIntegration.Infra.SeedWork
{
    public class SnakeCaseNamingPolicy  : JsonNamingPolicy
    {
        private readonly SnakeCaseNamingStrategy _newtonsoftSnakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

        public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

        public override string ConvertName(string name)
        {
            return _newtonsoftSnakeCaseNamingStrategy.GetPropertyName(name, false);
        }
    }
}