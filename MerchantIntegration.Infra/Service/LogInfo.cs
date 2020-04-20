using System;
using MerchantIntegration.Core.Contracts.Infrastruture.Service;
using ILogger = Serilog.ILogger;

namespace MerchantIntegration.Infra.Service
{
    public class LogInfo : ILogInfo
    {
        private readonly ILogger _log;

        public LogInfo(Serilog.ILogger log)
        {
            _log = log;
        }

        public void InfoMessage<T>(string message, T objectValue) where T : class
        {
            var objectName = objectValue.GetType().Name;
            var messageString = "message, {@"+objectName+"}!";
            _log.Information(messageString, objectValue);
        }
    }
}