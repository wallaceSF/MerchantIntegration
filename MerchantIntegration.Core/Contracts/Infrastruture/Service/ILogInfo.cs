using System.Threading.Tasks;

namespace MerchantIntegration.Core.Contracts.Infrastruture.Service
{
    public interface ILogInfo
    {
        void InfoMessage<T>(string message, T objectValue) where T : class;
    }
}