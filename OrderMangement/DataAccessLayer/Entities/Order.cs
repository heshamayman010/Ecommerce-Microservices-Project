using MongoDB.Bson.Serialization.Attributes;

namespace OrderMangement.DataAccessLayer.Entities;

public class Order
{

  // binary json : BsonRepresentation and we use it to make the data not stored as binary format 
  [BsonId]
  [BsonRepresentation(MongoDB.Bson.BsonType.String)]
  public Guid _id {  get; set; }

  [BsonRepresentation(MongoDB.Bson.BsonType.String)]
  public Guid OrderID { get; set; }

  [BsonRepresentation(MongoDB.Bson.BsonType.String)]
  public Guid UserID { get; set; }

  [BsonRepresentation(MongoDB.Bson.BsonType.String)]
  public DateTime OrderDate { get; set; }

  [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
  public decimal TotalBill {  get; set; }

  public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
