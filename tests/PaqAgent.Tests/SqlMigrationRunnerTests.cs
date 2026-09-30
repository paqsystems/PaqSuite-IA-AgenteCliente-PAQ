using System.Reflection;
using PaqAgent.Database;

namespace PaqAgent.Tests;

public sealed class SqlMigrationRunnerTests
{
    [Fact]
    public void SplitSqlBatches_SeparaGoEnLinea()
    {
        var batches = SqlMigrationRunner.SplitSqlBatches(
            "CREATE PROCEDURE dbo.Test AS SELECT 1;\nGO\nCREATE PROCEDURE dbo.Test2 AS SELECT 2;\nGO 2");

        Assert.Equal(2, batches.Count);
        Assert.Contains("dbo.Test", batches[0]);
        Assert.Contains("dbo.Test2", batches[1]);
    }

    [Fact]
    public void ComputeSha256Hex_DevuelveHexadecimalMayuscula()
    {
        var checksum = SqlMigrationRunner.ComputeSha256Hex("abc");

        Assert.Equal("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", checksum);
    }

    [Fact]
    public void SqlScriptLoader_LeeRecursosDictionaryYCompany()
    {
        var scripts = new SqlScriptLoader().Load(Assembly.GetAssembly(typeof(SqlScriptLoader))!);

        Assert.Contains(scripts, script => script.Phase == "dictionary");
        Assert.Contains(scripts, script => script.Phase == "company");
        Assert.All(scripts, script => Assert.NotEmpty(script.Content));
    }
}
