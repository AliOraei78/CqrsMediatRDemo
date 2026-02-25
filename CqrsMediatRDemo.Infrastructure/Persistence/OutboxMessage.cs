namespace CqrsMediatRDemo.Infrastructure.Persistence;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;          // Full name of event type
    public string Payload { get; set; } = string.Empty;       // JSON serialized event
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedOn { get; set; }
    public int AttemptCount { get; set; } = 0;
    public string? Error { get; set; }
}