namespace OrderMangement.BusinessLogicLayer.DTO;

public class ProductDto
{
    
public Guid ProductID { get; set; }
public string? ProductName { get; set; }
public string? Category { get; set; }
public double UnitPrice { get; set; }
public double QuantityInStock { get; set; }


}
