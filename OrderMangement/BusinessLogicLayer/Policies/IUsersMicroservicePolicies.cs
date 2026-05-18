
using Polly;

namespace  OrderMangement.BusinessLogicLayer.Ploicies;

public interface IUsersMicroservicePolicies
{
  IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
}
