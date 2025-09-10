using System.Collections.Generic;
using MunicipalityApp.Models;

namespace MunicipalityApp.Services;

public sealed class IssueStore : IIssueStore
{
    private readonly LinkedList<IssueReport> _issues = new(); // no List/array
    private readonly SortedDictionary<string, int> _categoryCounts =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, int> CategoryCounts => _categoryCounts;

    public IssueReport Add(IssueReport issue)
    {
        _issues.AddFirst(issue); // O(1) insert
        if (_categoryCounts.ContainsKey(issue.Category))
            _categoryCounts[issue.Category]++;
        else
            _categoryCounts[issue.Category] = 1;
        return issue;
    }

    public IEnumerable<IssueReport> All() => _issues;

    public bool TryGet(Guid id, out IssueReport? issue)
    {
        foreach (var it in _issues)
        {
            if (it.Id == id) { issue = it; return true; }
        }
        issue = null;
        return false;
    }
}
