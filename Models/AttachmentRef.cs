namespace MunicipalityApp.Models;

public sealed class AttachmentRef
{
    public string FileName { get; init; } = "";
    public string StoredPath { get; init; } = "";
    public long SizeBytes { get; init; }
}
