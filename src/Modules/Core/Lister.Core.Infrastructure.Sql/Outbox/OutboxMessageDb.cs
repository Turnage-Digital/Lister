namespace Lister.Core.Infrastructure.Sql.Outbox;

public class OutboxMessageDb
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
}