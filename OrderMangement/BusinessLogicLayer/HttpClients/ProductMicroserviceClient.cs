using System.Net.Http.Json;
using OrderMangement.BusinessLogicLayer.DTO;
namespace OrderMangement.BusinessLogicLayer.HttpClients;

public class ProductMicroserviceClient
{
    private readonly HttpClient _httpClient;

  public   ProductMicroserviceClient(HttpClient httpClient)
    {
        _httpClient=httpClient;


    }


    public async Task<ProductDto?>GetProductByProductID(Guid ProductId)
    {
        
      HttpResponseMessage response=await  _httpClient.GetAsync($"/api/products/search/product-id/{ProductId}");

    if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        // we must use the read from json async 
var product=  response.Content.ReadFromJsonAsync<ProductDto>();

        if (product == null)
        {
            throw new ArgumentException("Product Cant be found ");

        }
        
        return await product; 

    }



}