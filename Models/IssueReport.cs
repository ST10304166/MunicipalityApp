namespace MunicipalityApp.Models;

public enum IssueStatus { Submitted, Assigned, InProgress, Resolved }

public sealed class IssueReport
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    // Form fields
    public string Location { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";

    // Attachments (FIFO)
    public Queue<AttachmentRef> Attachments { get; } = new();

    // Status for transparency/feedback
    public IssueStatus Status { get; set; } = IssueStatus.Submitted;
}
