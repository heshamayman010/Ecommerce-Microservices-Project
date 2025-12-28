using ProductManagement.Core.Dtos;
using System.Linq.Expressions;
using UserManagement.Infrastructure.Entities;

namespace ProductManagement.Core.IServices;

public interface IProductsService
{
  Task<List<ProductResponse?>> GetProducts();
  Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
  Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
  Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);
  Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);
  Task<bool> DeleteProduct(Guid productID);
}
