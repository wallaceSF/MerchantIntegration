using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace MerchantIntegration.Infra.SeedWork
{
    public class SnakeJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            //PascalCase to snake_case
            return string.Concat(clrPropertyName.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? "_" + char.ToLower(x).ToString() : char.ToLower(x).ToString()));
        }
        
        protected override bool TrySerializeUnknownTypes(object input, out object output)
        {
            bool returnValue = base.TrySerializeUnknownTypes(input, out output);

            if (output is IDictionary<string, object> obj)
            {
                output = obj.Where(o => o.Value != null).ToDictionary(o => o.Key, o => o.Value);
            }

            return false;
        }
    }
}