namespace ProductManagement.Core.RabbitMQ;

public record ProductDeletionMessage(Guid ProductID, string? ProductName);
