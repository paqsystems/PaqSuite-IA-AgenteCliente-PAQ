namespace PaqAgent.Options;

public sealed class AgentOptions
{
    public string AgentId { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string AgentToken { get; set; } = "";
    public string GatewayUrl { get; set; } = "";
    public SqlOptions Sql { get; set; } = new();
    public SqlMigrationOptions SqlMigrations { get; set; } = new();

    public bool HasRequiredIdentity =>
        !string.IsNullOrWhiteSpace(AgentId)
        && !string.IsNullOrWhiteSpace(ClientId)
        && !string.IsNullOrWhiteSpace(AgentToken)
        && !string.IsNullOrWhiteSpace(GatewayUrl);

    public bool HasSqlConfig =>
        !string.IsNullOrWhiteSpace(Sql.Server)
        && !string.IsNullOrWhiteSpace(Sql.Database);
}

public sealed class SqlMigrationOptions
{
    public bool Enabled { get; set; } = true;
    public int CommandTimeoutSeconds { get; set; } = 30;
}

public sealed class SqlOptions
{
    public string Server { get; set; } = "";
    public string Database { get; set; } = "";
    public string User { get; set; } = "";
    public string Password { get; set; } = "";

    /// <summary>Opcional. Si tiene valor y Server no es named instance (sin '\'), se usa host,port.</summary>
    public int? Port { get; set; }

    /// <summary>Lab SSMS “Opcional” ⇒ false. Producción: true.</summary>
    public bool Encrypt { get; set; }

    public bool TrustServerCertificate { get; set; } = true;
}
