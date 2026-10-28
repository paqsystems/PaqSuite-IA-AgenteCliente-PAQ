using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using PaqAgent.Database;
using PaqAgent.Installation;
using PaqAgent.Options;
using PaqContracts;

namespace PaqAgent.Tests;

public sealed class InstallationGatewayRunnerTests
{
    [Fact]
    public async Task RunInstallStep_FaltaPackRoot()
    {
        var runner = new InstallationGatewayRunner(new RecordingSqlExecutor(), NullLogger<InstallationGatewayRunner>.Instance);
        var options = new AgentOptions
        {
            Sql = new SqlOptions { Server = "localhost", Database = "dict" }
        };
        var parameters = MinimalStepParameters("scripts/test.sql");

        var outcome = await runner.RunInstallOrUpdateStepAsync(options, parameters, 30, CancellationToken.None);

        Assert.Equal(JobStatuses.Failed, outcome.Status);
        Assert.Equal("INSTALLATION_PACK_ROOT_MISSING", outcome.ErrorCode);
    }

    [Fact]
    public async Task RunInstallStep_AplicaSqlEnDictionary()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "paq-install-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var relativePath = "sql/demo.sql";
        var absolute = Path.Combine(tempDir, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
        const string sql = "SELECT 1;";
        await File.WriteAllTextAsync(absolute, sql, Encoding.UTF8);
        var checksum = Convert
            .ToHexString(System.Security.Cryptography.SHA256.HashData(await File.ReadAllBytesAsync(absolute)))
            .ToLowerInvariant();

        var executor = new RecordingSqlExecutor();
        var runner = new InstallationGatewayRunner(executor, NullLogger<InstallationGatewayRunner>.Instance);
        var options = new AgentOptions
        {
            Sql = new SqlOptions { Server = "localhost", Database = "dict" },
            Installation = new InstallationOptions { PackRoot = tempDir }
        };

        var parameters = BuildStepParameters(relativePath, checksum);
        var outcome = await runner.RunInstallOrUpdateStepAsync(options, parameters, 30, CancellationToken.None);

        Assert.Equal(JobStatuses.Success, outcome.Status);
        Assert.True(executor.NonQueries.Count >= 2);
        try
        {
            Directory.Delete(tempDir, recursive: true);
        }
        catch
        {
            // best effort cleanup
        }
    }

    [Fact]
    public async Task RunCompanyProvision_InitialSinSqlConfig_Degraded()
    {
        var runner = new InstallationGatewayRunner(new RecordingSqlExecutor(), NullLogger<InstallationGatewayRunner>.Instance);
        var outcome = await runner.RunCompanyProvisionAsync(
            new AgentOptions(),
            new Dictionary<string, object?> { ["mode"] = "initial" },
            30,
            CancellationToken.None);

        Assert.Equal(JobStatuses.Degraded, outcome.Status);
        Assert.Equal("SQL_NOT_CONFIGURED", outcome.ErrorCode);
    }

    private static Dictionary<string, object?> MinimalStepParameters(string path)
    {
        return BuildStepParameters(path, "0000000000000000000000000000000000000000000000000000000000000000");
    }

    private static Dictionary<string, object?> BuildStepParameters(string path, string checksum)
    {
        var step = new Dictionary<string, object?>
        {
            ["id"] = "step-demo",
            ["kind"] = "sql",
            ["target"] = "dictionary",
            ["path"] = path,
            ["checksum"] = checksum
        };
        var artifact = new Dictionary<string, object?>
        {
            ["path"] = path,
            ["checksum"] = checksum,
            ["bytes"] = 10
        };

        return new Dictionary<string, object?>
        {
            ["step"] = JsonSerializer.SerializeToElement(step),
            ["artifact"] = JsonSerializer.SerializeToElement(artifact),
            ["runId"] = "run-1",
            ["manifestId"] = "manifest-1"
        };
    }

    private sealed class RecordingSqlExecutor : ISqlExecutor
    {
        public List<string> NonQueries { get; } = new();

        public Task ExecuteNonQueryAsync(
            string connectionString,
            string sql,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            NonQueries.Add(sql);
            return Task.CompletedTask;
        }

        public Task<DataTable> QueryAsync(
            string connectionString,
            string sql,
            int commandTimeoutSeconds,
            CancellationToken cancellationToken)
        {
            var table = new DataTable();
            if (sql.Contains("paq_sp_migrations", StringComparison.Ordinal)
                && sql.Contains("SELECT migration", StringComparison.Ordinal))
            {
                table.Columns.Add("migration");
                table.Columns.Add("checksum_sha256");
            }
            else if (sql.Contains("next_batch", StringComparison.Ordinal))
            {
                table.Columns.Add("next_batch");
                table.Rows.Add(1);
            }

            return Task.FromResult(table);
        }
    }
}
