namespace ProductManagement.Core.RabbitMQ;

public record ProductNameUpdateMessage(Guid ProductID, string? NewName);
