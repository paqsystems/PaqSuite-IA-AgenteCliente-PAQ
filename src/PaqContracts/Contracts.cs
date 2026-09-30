namespace PaqContracts;

/// <summary>Estados de job del contrato MVP (D12).</summary>
public static class JobStatuses
{
    public const string Success = "success";
    public const string Failed = "failed";
    public const string Timeout = "timeout";
    public const string Offline = "offline";
    public const string Degraded = "degraded";
    public const string Cancelled = "cancelled";
}

/// <summary>Estados de presencia del agente (GET /internal/agents/{id}/status).</summary>
public static class AgentPresenceStatuses
{
    public const string Online = "online";
    public const string Offline = "offline";
    public const string Degraded = "degraded";
}

public static class ErrorCodes
{
    public const string AgentOffline = "AGENT_OFFLINE";
    public const string AgentTimeout = "AGENT_TIMEOUT";
    public const string AgentAuthFailed = "AGENT_AUTH_FAILED";
}

/// <summary>Nombres de métodos hub Agent↔Gateway (M9).</summary>
public static class HubMethodNames
{
    public const string ExecuteJob = "ExecuteJob";
    public const string CompleteJob = "CompleteJob";
    public const string Heartbeat = "Heartbeat";
}

/// <summary>Operaciones de lista blanca MVP.</summary>
public static class JobOperations
{
    public const string DiagnosticsRun = "diagnostics.run";
    public const string AuthLogin = "auth.login";
    public const string AuthChangePassword = "auth.changePassword";
    public const string MenuAuthorized = "menu.authorized";
    public const string ClientesBuscar = "clientes.buscar";
    public const string ClientesObtener = "clientes.obtener";
    public const string ArticulosBuscar = "articulos.buscar";
    public const string ArticulosObtener = "articulos.obtener";
    public const string StockConsultar = "stock.consultar";
    public const string SaldosConsultar = "saldos.consultar";
    public const string PedidosPendientes = "pedidos.pendientes";
    public const string ComprobantesRecientes = "comprobantes.recientes";
    public const string TangoVersion = "tango.version";
    public const string ParametrosUpdate = "parametros.update";
}

public sealed class JobRequest
{
    public string TraceId { get; set; } = "";
    public string JobId { get; set; } = "";
    public string AgentId { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string Operation { get; set; } = "";
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}

public sealed class JobResult
{
    public string TraceId { get; set; } = "";
    public string JobId { get; set; } = "";
    public string Status { get; set; } = "";
    public object? Data { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public long DurationMs { get; set; }
}

/// <summary>Heartbeat con extension points de versión (plan SQL fase 2).</summary>
public sealed class AgentHeartbeat
{
    public string AgentId { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string AgentVersion { get; set; } = "";
    public string? SchemaVersionApplied { get; set; }
    public string Readiness { get; set; } = "network_ok";
    public DateTimeOffset TimestampUtc { get; set; }
}

public sealed class AgentStatusResponse
{
    public string AgentId { get; set; } = "";
    public string Status { get; set; } = AgentPresenceStatuses.Offline;
    public DateTimeOffset? LastSeenAt { get; set; }
    public string? LastSeenIp { get; set; }
}

/// <summary>Heartbeat 30 s / TTL online 90 s (H8).</summary>
public static class AgentDefaults
{
    public const int HeartbeatSeconds = 30;
    public const int OnlineTtlSeconds = 90;
}
