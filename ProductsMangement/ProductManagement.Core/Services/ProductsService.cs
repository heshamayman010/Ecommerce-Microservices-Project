using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ProductManagement.Core.Dtos;
using ProductManagement.Core.IServices;
using ProductManagement.Core.RabbitMQ;
using System.Linq.Expressions;
using UserManagement.Infrastructure;
using UserManagement.Infrastructure.Entities;
namespace ProductManagement.Core.Services;

public class ProductsService : IProductsService
{
  private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
  private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
  private readonly IMapper _mapper;
  private readonly IProductsRepository _productsRepository;
  private readonly IRabbitMQPublisher _rabbitMQPublisher;


  public ProductsService(IValidator<ProductAddRequest> productAddRequestValidator,IRabbitMQPublisher rabbitMQPublisher ,IValidator<ProductUpdateRequest> productUpdateRequestValidator, IMapper mapper, IProductsRepository productsRepository)
  {
    _productAddRequestValidator = productAddRequestValidator;
    _productUpdateRequestValidator = productUpdateRequestValidator;
    _mapper = mapper;
    _productsRepository = productsRepository;
        _rabbitMQPublisher = rabbitMQPublisher;

  }


  public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
  {
    if (productAddRequest == null)
    {
      throw new ArgumentNullException(nameof(productAddRequest));
    }

    ValidationResult validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

    if (!validationResult.IsValid)
    {
      string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); 
    }


    Product productInput = _mapper.Map<Product>(productAddRequest); 
    Product? addedProduct = await _productsRepository.AddProduct(productInput);

    if (addedProduct == null)
    {
      return null;
    }

    ProductResponse addedProductResponse = _mapper.Map<ProductResponse>(addedProduct); 
    return addedProductResponse;
  }


  public async Task<bool> DeleteProduct(Guid productID)
  {
    Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductID == productID);

    if (existingProduct == null)
    {
      return false;
    }

    bool isDeleted = await _productsRepository.DeleteProduct(productID);

    if (isDeleted)
    {
      ProductDeletionMessage message = new ProductDeletionMessage(existingProduct.ProductID, existingProduct.ProductName);
      //string routingKey = "product.delete";

      var headers = new Dictionary<string, object>() 
      {
        { "event", "product.delete" },
        { "RowCount", 1 }
      };

      _rabbitMQPublisher.Publish(headers, message);
    }


    return isDeleted;
  }


  public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
  {
    Product? product = await _productsRepository.GetProductByCondition(conditionExpression);
    if (product == null)
    {
      return null;
    }

    ProductResponse productResponse = _mapper.Map<ProductResponse>(product);
    return productResponse;
  }


  public async Task<List<ProductResponse?>> GetProducts()
  {
    IEnumerable<Product?> products = await _productsRepository.GetProducts();
    

    IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); 
    return productResponses.ToList();
  }


  public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
  {
    IEnumerable<Product?> products = await _productsRepository.GetProductsByCondition(conditionExpression);

    IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); 
    return productResponses.ToList();
  }


  public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
  {
    Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductID == productUpdateRequest.ProductID);

    if(existingProduct == null)
    {
      throw new ArgumentException("Invalid Product ID");
    }


    ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

    // Check the validation result
    if (!validationResult.IsValid)
    {
      string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); 
      throw new ArgumentException(errors);
    }


    bool isProductNameChanged = productUpdateRequest.ProductName != existingProduct.ProductName;
    Product product = _mapper.Map<Product>(productUpdateRequest); 

    Product? updatedProduct = await _productsRepository.UpdateProduct(product);

        if (isProductNameChanged)
    {
      var message = new ProductNameUpdateMessage(product.ProductID, product.ProductName);

      var headers = new Dictionary<string, object>()
      {
        { "event", "product.update" },
        { "field", "name" },
        { "RowCount", 1 }
      };
            _rabbitMQPublisher.Publish<ProductNameUpdateMessage>(headers, message);

    }



    ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct);

    return updatedProductResponse;
  }
}
