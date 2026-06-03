using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper.Execution;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrderMangement.BusinessLogicLayer.DTO;
using Polly.Bulkhead;
namespace OrderMangement.BusinessLogicLayer.HttpClients;

public class ProductMicroserviceClient
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<ProductMicroserviceClient> _logger;

    public ProductMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache,
     ILogger<ProductMicroserviceClient> logger)
    {
        _httpClient = httpClient;
        _distributedCache = distributedCache;
        _logger = logger;
    }


    public async Task<ProductDto?> GetProductByProductID(Guid ProductId)
    {


        try
        {
            // first we need to check if it is in the cach or not 

            var KeyForCahce = $"Product:{ProductId}";
            var ProductFromCache = await _distributedCache.GetStringAsync(KeyForCahce);

            if (ProductFromCache != null)
            {

                var productToReturn = JsonSerializer.Deserialize<ProductDto>(ProductFromCache);
                return productToReturn;
            }

            // now if the product is not in the cache so we need to get it from the product service and then store it 
            // in the cache 

            HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{ProductId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {// this one is for the fall back function to make it not store the data in the cach as it is dummy data 
                    var productForFallBack = await response.Content.ReadFromJsonAsync<ProductDto>();

                    if (productForFallBack == null)
                    {
                        throw new ArgumentException("Fall Back Function is not implemented  ");

                    }
                    else
                    {
                        return  productForFallBack;
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
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }

            // we must use the read from json async 
            var product =await  response.Content.ReadFromJsonAsync<ProductDto>();

            if (product == null)
            {
                throw new ArgumentException("Product Cant be found ");

            }

            // now we need to store the product we got 
            // first we need to make the key then serialize the product to stroe it as json then create 
            // the cache options to use 

            var productToCache = JsonSerializer.Serialize(product);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            // now store the value 

            await _distributedCache.SetStringAsync(KeyForCahce, productToCache, options);


            return  product;

        }
        catch (BulkheadRejectedException ex)
        {
            _logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");

            return new ProductDto()
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Temporarily Unavailable (Bulkhead)",
                Category = "Temporarily Unavailable (Bulkhead)",
                UnitPrice = 0,
                QuantityInStock = 0
            };

        }

    }



}