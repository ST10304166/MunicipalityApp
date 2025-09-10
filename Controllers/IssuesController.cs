using Microsoft.AspNetCore.Mvc;
using MunicipalityApp.Models;
using MunicipalityApp.Services;

namespace MunicipalityApp.Controllers;

public class IssuesController : Controller
{
    private readonly IIssueStore _store;
    private readonly IWebHostEnvironment _env;

    public IssuesController(IIssueStore store, IWebHostEnvironment env)
    {
        _store = store;
        _env = env;
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = GetCategories();
        return View(new IssueInput());
    }

    [HttpPost]
    [RequestSizeLimit(20_000_000)] // 20 MB total
    public async Task<IActionResult> Create(IssueInput input, List<IFormFile> files)
    {
        ViewBag.Categories = GetCategories();
        if (!ModelState.IsValid) return View(input);

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".heic" };

        const long maxEach = 10_000_000; // 10 MB per file (defensive)
        const long maxTotal = 20_000_000; // 20 MB per request
        long total = 0;

        // Validate files before saving
        foreach (var f in files ?? Enumerable.Empty<IFormFile>())
        {
            if (f.Length <= 0) continue;
            var ext = Path.GetExtension(f.FileName);
            if (!allowed.Contains(ext))
                ModelState.AddModelError(string.Empty, $"File type not allowed: {ext}");
            total += f.Length;
            if (f.Length > maxEach)
                ModelState.AddModelError(string.Empty, $"{f.FileName} exceeds 10 MB");
            if (total > maxTotal)
                ModelState.AddModelError(string.Empty, "Total upload exceeds 20 MB");
        }

        if (!ModelState.IsValid) return View(input);

        var report = new IssueReport
        {
            Location = input.Location.Trim(),
            Category = input.Category.Trim(),
            Description = input.Description.Trim(),
            Status = IssueStatus.Submitted
        };

        var today = DateTime.UtcNow;
        var folder = Path.Combine(_env.WebRootPath, "uploads",
            today.ToString("yyyy"), today.ToString("MM"), today.ToString("dd"));
        Directory.CreateDirectory(folder);

        foreach (var f in files ?? Enumerable.Empty<IFormFile>())
        {
            if (f.Length <= 0) continue;
            var safeName = Path.GetFileName(f.FileName);
            var storedName = $"{Guid.NewGuid()}_{safeName}";
            var fullPath = Path.Combine(folder, storedName);

            using var stream = System.IO.File.Create(fullPath);
            await f.CopyToAsync(stream);

            var publicPath = $"/uploads/{today:yyyy}/{today:MM}/{today:dd}/{storedName}";
            report.Attachments.Enqueue(new AttachmentRef
            {
                FileName = safeName,
                StoredPath = publicPath,
                SizeBytes = f.Length
            });
        }

        _store.Add(report);
        TempData["Success"] = "Thank you! Your report was submitted.";
        return RedirectToAction(nameof(Thanks), new { id = report.Id });
    }


    [HttpGet]
    public IActionResult Thanks(Guid id)
    {
        if (_store.TryGet(id, out var issue))
            return View(issue);

        TempData["Error"] = "We couldn't find that report.";
        return RedirectToAction(nameof(Create));
    }

    private static IEnumerable<string> GetCategories() => new[]
    {
        "Sanitation", "Roads", "Water", "Electricity", "Refuse Collection", "Other"
    };
}
