using System.ComponentModel.DataAnnotations;

namespace MunicipalityApp.Models;

public sealed class IssueInput
{
    [Required, StringLength(200, MinimumLength = 3)]
    public string Location { get; set; } = "";

    [Required]
    public string Category { get; set; } = "";

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = "";
}
