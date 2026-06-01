using Polly;

namespace  OrderMangement.BusinessLogicLayer.Ploicies;

public interface IProductsMicroservicePolicies
{  IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
  IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy();
}
