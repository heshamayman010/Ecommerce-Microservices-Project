using System.Net.Http.Json;
using OrderMangement.BusinessLogicLayer.DTO;
namespace OrderMangement.BusinessLogicLayer.HttpClients;

public class UserMicroserviceClient
{
    private readonly HttpClient _httpClient;

  public   UserMicroserviceClient(HttpClient httpClient)
    {
        _httpClient=httpClient;


    }


  public async Task<UserDTO?> GetUserByUserID(Guid userID)
  {
    HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");

    if (!response.IsSuccessStatusCode)
    {
      if (response.StatusCode == System.Net.HttpStatusCode.NotFound) 
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
        return new UserDTO(){
       PersonName   = "Temporarily Unavailable",
          Email= "Temporarily Unavailable",
          Gender= "Temporarily Unavailable",
          UserID= Guid.Empty};
      }
    }


    UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

    if (user == null) 
    {
      throw new ArgumentException("Invalid User ID");
    }

    return user;
  }
}


