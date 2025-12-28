namespace ProductManagement.Core.Dtos;

public record ProductResponse(Guid ProductID, string ProductName, CategoryOptions Category, double? UnitPrice, int? QuantityInStock)
{
  public ProductResponse() : this(default, default, default, default, default)
  {
  }
}
