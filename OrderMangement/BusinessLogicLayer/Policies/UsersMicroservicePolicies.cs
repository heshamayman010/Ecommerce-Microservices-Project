using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderMangement.BusinessLogicLayer.DTO;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace  OrderMangement.BusinessLogicLayer.Ploicies;

public class UsersMicroservicePolicies : IUsersMicroservicePolicies
{
  private readonly ILogger<UsersMicroservicePolicies> _logger;

  public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger)
  {
    _logger = logger;
  }


  public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
  {
    AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
  .WaitAndRetryAsync(
     retryCount: 5, //Number of retries
     sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Delay between retries
     onRetry: (outcome, timespan, retryAttempt, context) =>
     {
       _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
     });

    return policy;
  }


  public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
  {
    AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
  .CircuitBreakerAsync(
     handledEventsAllowedBeforeBreaking: 20, 
     durationOfBreak: TimeSpan.FromMinutes(2), 
     onBreak: (outcome, timespan) =>
     {
       _logger.LogInformation($"Circuit breaker opened for {timespan.TotalMinutes} minutes due to consecutive 3 failures");
     },
     onReset: () => {
       _logger.LogInformation($"Circuit breaker closed. The subsequent requests will be allowed.");
     });

    return policy;
  }


  public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
  {
    AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMinutes(2));

    return policy;
  }

  public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
  {
    AsyncFallbackPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
      .FallbackAsync(async (context) =>
      {
        _logger.LogWarning("Fallback triggered: The request failed, returning dummy data");

        UserDTO user = new UserDTO(){
          UserID= Guid.Empty,
          Email = "Temporarily Unavailable (fallback)",
          PersonName= "Temporarily Unavailable (fallback)",
          Gender=  "Temporarily Unavailable (fallback)",
          }
          ;

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
        {
          Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
        };

        return response;
      });

    return policy;
  }

  public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
  {

    var retryPolicy = GetRetryPolicy();
    var circuitBreakerPolicy = GetCircuitBreakerPolicy();
    var timeoutPolicy = GetTimeoutPolicy();
    var fallbackPolicy = GetFallbackPolicy();
    
    var wrappedPolicy = Policy.WrapAsync(
      fallbackPolicy,      
      timeoutPolicy,       
      circuitBreakerPolicy,  
      retryPolicy         
    );

    return wrappedPolicy;
  }
}
