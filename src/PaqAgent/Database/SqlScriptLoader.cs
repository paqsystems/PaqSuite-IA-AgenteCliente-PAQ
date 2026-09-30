using System.Reflection;

namespace PaqAgent.Database;

public sealed record SqlMigrationScript(string Migration, string Phase, string Content);

public sealed class SqlScriptLoader
{
    private const string ResourcePrefix = ".Sql.";

    public IReadOnlyList<SqlMigrationScript> Load(Assembly? assembly = null)
    {
        assembly ??= typeof(SqlScriptLoader).Assembly;

        return assembly.GetManifestResourceNames()
            .Where(name => name.Contains(ResourcePrefix, StringComparison.OrdinalIgnoreCase)
                && name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .Select(name => CreateScript(assembly, name))
            .OrderBy(script => script.Phase, StringComparer.Ordinal)
            .ThenBy(script => script.Migration, StringComparer.Ordinal)
            .ToArray();
    }

    private static SqlMigrationScript CreateScript(Assembly assembly, string resourceName)
    {
        var marker = resourceName.IndexOf(ResourcePrefix, StringComparison.OrdinalIgnoreCase);
        var relative = resourceName[(marker + ResourcePrefix.Length)..];
        var parts = relative.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            throw new InvalidOperationException($"Recurso SQL inválido: {resourceName}");
        }

        var phase = parts[0];
        var migration = string.Join('.', parts.Skip(1));
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"No se pudo leer el recurso SQL: {resourceName}");
        using var reader = new StreamReader(stream);

        return new SqlMigrationScript(migration, phase, reader.ReadToEnd());
    }
}
