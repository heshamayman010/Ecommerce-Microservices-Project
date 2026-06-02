using AutoMapper;
using OrderMangement.BusinessLogicLayer.DTO;
using OrderMangement.BusinessLogicLayer.ServiceContracts;
using OrderMangement.DataAccessLayer.Entities;
using OrderMangement.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;
using OrderMangement.BusinessLogicLayer.HttpClients;

namespace OrderMangement.BusinessLogicLayer.Services;

public class OrdersService : IOrdersService
{
  private readonly IValidator<OrderAddRequest> _orderAddRequestValidator;
  private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
  private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
  private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
  private readonly IMapper _mapper;
  private IOrdersRepository _ordersRepository;

  private UserMicroserviceClient _usermicorserviceclient;
  private ProductMicroserviceClient _ProductMicroserviceClient;


  public OrdersService(ProductMicroserviceClient ProductMicroserviceClient,UserMicroserviceClient userMicroserviceClient, IOrdersRepository ordersRepository, IMapper mapper, IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderItemAddRequest> orderItemAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator)
  {
    _orderAddRequestValidator = orderAddRequestValidator;
    _orderItemAddRequestValidator = orderItemAddRequestValidator;
    _orderUpdateRequestValidator = orderUpdateRequestValidator;
    _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
    _mapper = mapper;
    _ordersRepository = ordersRepository;
    _usermicorserviceclient = userMicroserviceClient;
    _ProductMicroserviceClient=ProductMicroserviceClient;
  }


  public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
  {
    //Check for null parameter
    if (orderAddRequest == null)
    {
      throw new ArgumentNullException(nameof(orderAddRequest));
    }


    ValidationResult orderAddRequestValidationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);
    if (!orderAddRequestValidationResult.IsValid)
    {
      string errors = string.Join(", ", orderAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
      throw new ArgumentException(errors);
    }

    // here we will check if the user id exist or not but it is from another microservices

    // get it back when working with users   hesham

    UserDTO? user = await _usermicorserviceclient.GetUserByUserID(orderAddRequest.UserID);

    if (user == null)
    {
      throw new ArgumentException("user is not found ");
    }


    //-----------------------

List<ProductDto>Products =new List<ProductDto>();

    foreach (OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
    {
      ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);

      if (!orderItemAddRequestValidationResult.IsValid)
      {
        string errors = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
        throw new ArgumentException(errors);
      }


      // now we need to check for the product is realy in the products or not using the product microservice 

     var product= await _ProductMicroserviceClient.GetProductByProductID(orderItemAddRequest.ProductID);
     if(product == null)
      {
        throw new ArgumentException("Cant find This Product");
      }
      Products.Add(product);
    }


    Order orderInput = _mapper.Map<Order>(orderAddRequest);

    foreach (OrderItem orderItem in orderInput.OrderItems)
    {
      orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
    }
    orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);


    Order? addedOrder = await _ordersRepository.AddOrder(orderInput);

    if (addedOrder == null)
    {
      return null;
    }

    OrderResponse addedOrderResponse = _mapper.Map<OrderResponse>(addedOrder);
    if (addedOrderResponse != null)
    {
      
    foreach (var orderitem in addedOrderResponse.OrderItems)
      {
        
        var orderproduct= Products.Where(x=>x.ProductID==orderitem.ProductID).FirstOrDefault();
      
        if (orderproduct == null)
        {
          continue;
        }
        _mapper.Map<ProductDto,OrderItemResponse>(orderproduct,orderitem);
      
      }
        if (user != null)
          {
            
            _mapper.Map<UserDTO,OrderResponse>(user,addedOrderResponse);
          }

    }
    


    
    return addedOrderResponse;
  }



  public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
  {
    if (orderUpdateRequest == null)
    {
      throw new ArgumentNullException(nameof(orderUpdateRequest));
    }


    ValidationResult orderUpdateRequestValidationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);
    if (!orderUpdateRequestValidationResult.IsValid)
    {
      string errors = string.Join(", ", orderUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
      throw new ArgumentException(errors);
    }


var Products =new List<ProductDto>();
    foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
    {
      ValidationResult orderItemUpdateRequestValidationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);

      if (!orderItemUpdateRequestValidationResult.IsValid)
      {
        string errors = string.Join(", ", orderItemUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
        throw new ArgumentException(errors);
      }

      // now we need to check for the product is realy in the products or not using the product microservice 

     var product= await _ProductMicroserviceClient.GetProductByProductID(orderItemUpdateRequest.ProductID);
     if(product == null)
      {
        throw new ArgumentException("Cant find This Product");
      }
Products.Add(product);

    }

    // here we will check if the user id exist or not but it is from another microservices 
    UserDTO? user = await _usermicorserviceclient.GetUserByUserID(orderUpdateRequest.UserID);

    if (user == null)
    
    {
      throw new ArgumentException("user is not found ");
    }



    //-----------------------


    Order orderInput = _mapper.Map<Order>(orderUpdateRequest);

    foreach (OrderItem orderItem in orderInput.OrderItems)
    {
      orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
    }
    orderInput.TotalBill = orderInput.OrderItems.Sum(temp => temp.TotalPrice);


    Order? updatedOrder = await _ordersRepository.UpdateOrder(orderInput);

    if (updatedOrder == null)
    {
      return null;
    }

    OrderResponse updatedOrderResponse = _mapper.Map<OrderResponse>(updatedOrder);
    if (updatedOrderResponse != null)
    {
      
    foreach (var orderitem in updatedOrderResponse.OrderItems)
      {
        
        var orderproduct= Products.Where(x=>x.ProductID==orderitem.ProductID).FirstOrDefault();
      
        if (orderproduct == null)
        {
          continue;
        }
        _mapper.Map<ProductDto,OrderItemResponse>(orderproduct,orderitem);
      
      }


    if (user != null)
          {
            
            _mapper.Map<UserDTO,OrderResponse>(user,updatedOrderResponse);
          }


    }
    

    return updatedOrderResponse;
  }


  public async Task<bool> DeleteOrder(Guid orderID)
  {
    FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, orderID);
    Order? existingOrder = await _ordersRepository.GetOrderByCondition(filter);

    if (existingOrder == null)
    {
      return false;
    }


    bool isDeleted = await _ordersRepository.DeleteOrder(orderID);
    return isDeleted;
  }


  public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
  {
    Order? order = await _ordersRepository.GetOrderByCondition(filter);
    if (order == null)
      return null;




    OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);
        foreach (var orderitem in orderResponse.OrderItems)
      {
        
        var orderproduct=await _ProductMicroserviceClient.GetProductByProductID(orderitem.ProductID);
      
        if (orderproduct == null)
        {
          continue;
        }
        _mapper.Map<ProductDto,OrderItemResponse>(orderproduct,orderitem);
      

      }
    var userback=await _usermicorserviceclient.GetUserByUserID(orderResponse.UserID);

    if (userback != null)
          {
            
            _mapper.Map<UserDTO,OrderResponse>(userback,orderResponse);
          }


    return orderResponse;
  }


  public async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
  {
    IEnumerable<Order?> orders = await _ordersRepository.GetOrdersByCondition(filter);


    IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);

      // now we need to load the product name an category 

      foreach(var orderr in orderResponses)
    {
      
      if (orderr == null)
      {
      continue;  
      }


// here we will use the mapped data from the getby condition as we will make it get bakc the data 
// of the product from the products microservices each time to be updated 
    foreach (var orderitem in orderr.OrderItems)
      {
        
        var orderproduct=await _ProductMicroserviceClient.GetProductByProductID(orderitem.ProductID);
      
        if (orderproduct == null)
        {
          continue;
        }
        _mapper.Map<ProductDto,OrderItemResponse>(orderproduct,orderitem);
      
      }
var userback=await _usermicorserviceclient.GetUserByUserID(orderr.UserID);

if (userback != null)
      {
        
        _mapper.Map<UserDTO,OrderResponse>(userback,orderr);
      }

    }
    


    return orderResponses.ToList();
  }


  public async Task<List<OrderResponse?>> GetOrders()
  {
    IEnumerable<Order?> orders = await _ordersRepository.GetOrders();


    IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);
    
      // now we need to load the product name an category 

      foreach(var orderr in orderResponses)
    {
      
      if (orderr == null)
      {
      continue;  
      }


// here we will use the mapped data from the getby condition as we will make it get bakc the data 
// of the product from the products microservices each time to be updated 
    foreach (var orderitem in orderr.OrderItems)
      {
        
        var orderproduct=await _ProductMicroserviceClient.GetProductByProductID(orderitem.ProductID);
      
        if (orderproduct == null)
        {
          continue;
        }
        _mapper.Map<ProductDto,OrderItemResponse>(orderproduct,orderitem);
      
      }
var userback=await _usermicorserviceclient.GetUserByUserID(orderr.UserID);

if (userback != null)
      {
        
        _mapper.Map<UserDTO,OrderResponse>(userback,orderr);
      }

    }
    

// now we want to add the data of the user for the orders 




    return orderResponses.ToList();
  }
}