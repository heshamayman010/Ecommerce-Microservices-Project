
using Polly;

namespace  OrderMangement.BusinessLogicLayer.Ploicies;

public interface IUsersMicroservicePolicies
{
  IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
  IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();
  IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy();
  IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
}
