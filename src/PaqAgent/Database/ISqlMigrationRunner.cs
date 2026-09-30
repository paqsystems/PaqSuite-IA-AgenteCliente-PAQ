using PaqAgent.Options;

namespace PaqAgent.Database;

public interface ISqlMigrationRunner
{
    Task RunAsync(AgentOptions options, CancellationToken cancellationToken);
}
