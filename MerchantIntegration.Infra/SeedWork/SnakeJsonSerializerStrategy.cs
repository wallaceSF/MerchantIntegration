using System.Linq;
using RestSharp;

namespace MerchantIntegration.Infra.SeedWork
{
    public class SnakeJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            //PascalCase to snake_case
            return string.Concat(clrPropertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + char.ToLower(x).ToString() : char.ToLower(x).ToString()));
        }
    }
}