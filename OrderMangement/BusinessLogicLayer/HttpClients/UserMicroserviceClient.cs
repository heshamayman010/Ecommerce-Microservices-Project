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


    public async Task<UserDTO?>GetUserByUserId(Guid userid)
    {
        
      HttpResponseMessage response=await  _httpClient.GetAsync($"/api/users/{userid}");

    if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        // we must use the read from json async 
var user=  response.Content.ReadFromJsonAsync<UserDTO>();

        if (user == null)
        {
            throw new ArgumentException("user Cant be found ");

        }
        
        return await user; 

    }



}