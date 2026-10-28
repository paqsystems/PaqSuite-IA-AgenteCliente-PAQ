using PaqContracts;

namespace PaqAgent.Installation;

public sealed class InstallationOutcome
{
    public string Status { get; init; } = JobStatuses.Failed;
    public object? Data { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }
}
