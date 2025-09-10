using MunicipalityApp.Models;

namespace MunicipalityApp.Services;

public interface IIssueStore
{
    IssueReport Add(IssueReport issue);
    bool TryGet(Guid id, out IssueReport? issue);
    IEnumerable<IssueReport> All(); // Enumerates LinkedList
    IReadOnlyDictionary<string, int> CategoryCounts { get; }
}
