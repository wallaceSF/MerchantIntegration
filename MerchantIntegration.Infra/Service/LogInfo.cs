using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using ILogger = Serilog.ILogger;

namespace MerchantIntegration.Infra.Service
{
    public class LogInfo : ILogInfo
    {
        private readonly ILogger _log;

        public LogInfo(ILogger log)
        {
            this._log = log;
        }

        public void InfoMessage<T>(T objectValue) where T : class
        {
            var objectName = objectValue.GetType().Name;
            var messageString = "message, {@"+objectName+"}!";
            this._log.Information(messageString, objectValue);
        }
    }
}