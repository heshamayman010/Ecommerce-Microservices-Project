using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderMangement.BusinessLogicLayer.DTO;
using Polly.CircuitBreaker;
using Polly.Timeout;
namespace OrderMangement.BusinessLogicLayer.HttpClients;

public class UserMicroserviceClient
{
  private readonly HttpClient _httpClient;
  private readonly IDistributedCache _distributedCache;
  private readonly ILogger<UserMicroserviceClient> _logger;

  public UserMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache, ILogger<UserMicroserviceClient> logger)
  {
    _httpClient = httpClient;
    _distributedCache = distributedCache;
    _logger = logger;



  }


  public async Task<UserDTO?> GetUserByUserID(Guid userID)
  {

    try
    {
      var KeyForCahce = $"user:{userID}";
      var userFromCache = await _distributedCache.GetStringAsync(KeyForCahce);
      if (userFromCache != null)
      {
        var UserToReturn = JsonSerializer.Deserialize<UserDTO>(userFromCache);
        return UserToReturn;
      }


      HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");

      if (!response.IsSuccessStatusCode)
      {
        if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        {// this one is for the fall back function to make it not store the data in the cach as it is dummy data 
          var userForFallBack = response.Content.ReadFromJsonAsync<UserDTO>();

          if (userForFallBack == null)
          {
            throw new ArgumentException("Fall Back Function is not implemented  ");

          }
          else
          {
            return await userForFallBack;
          }
        }

        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
          return null;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
          throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
        }
        else
        {
          //throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
          return new UserDTO()
          {
            PersonName = "Temporarily Unavailable",
            Email = "Temporarily Unavailable",
            Gender = "Temporarily Unavailable",
            UserID = Guid.Empty
          };
        }
      }


      UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

      if (user == null)
      {
        throw new ArgumentException("Invalid User ID");
      }

      // now we need to store the user data in the cache 
      var userToCache = JsonSerializer.Serialize(user);
      DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
      .SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(10))
      .SetSlidingExpiration(TimeSpan.FromMinutes(5));
      // now store the value 
      await _distributedCache.SetStringAsync(KeyForCahce, userToCache, options);
      return user;
    }

    catch (BrokenCircuitException ex)
    {
      _logger.LogError(ex, "Request failed because of circuit breaker is in Open state. Returning dummy data.");

      return new UserDTO()
      {
        PersonName = "Temporarily Unavailable (circuit breaker)",
        Email = "Temporarily Unavailable (circuit breaker)",
        Gender = "Temporarily Unavailable (circuit breaker)",
        UserID = Guid.Empty
      };
    }

    catch (TimeoutRejectedException ex)
    {
      _logger.LogError(ex, "Timeout occurred while fetching user data. Returning dummy data");

      return new UserDTO()
      {
        PersonName = "Temporarily Unavailable (timeout)",
        Email = "Temporarily Unavailable (timeout)",
        Gender = "Temporarily Unavailable (timeout)",
        UserID = Guid.Empty
      };
    }

  }


}


