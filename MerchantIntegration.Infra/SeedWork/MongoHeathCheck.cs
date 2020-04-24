using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MerchantIntegration.Infra.SeedWork
{
    public class MongoHeathCheck : IHealthCheck
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoHeathCheck(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        ) {
            try
            {
                var command = new BsonDocument {{"dbstats", 1}};
                var ok = _mongoDatabase.RunCommand<BsonDocument>(command).GetElement("ok").Value;

                if (ok.ToString() != "1")
                {
                    throw new Exception("Connection off");
                }
            }
            catch (DbException ex)
            {
                return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
            }

            return HealthCheckResult.Healthy();
        }
    }
}