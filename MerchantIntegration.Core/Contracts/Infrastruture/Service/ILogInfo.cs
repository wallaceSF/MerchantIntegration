namespace MerchantIntegration.Core.Contracts.Infrastruture.Service
{
    public interface ILogInfo
    {
        void InfoMessage<T>(T objectValue) where T : class;
    }
}