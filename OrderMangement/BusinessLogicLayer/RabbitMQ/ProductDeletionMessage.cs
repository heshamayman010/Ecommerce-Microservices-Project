namespace OrderMangement.BusinessLogicLayer.RabbitMQ;

public record ProductDeletionMessage(Guid ProductID, string? ProductName);
