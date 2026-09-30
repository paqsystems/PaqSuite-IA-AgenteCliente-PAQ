using System.Reflection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using PaqAgent.Acopios;
using PaqAgent.Articulos;
using PaqAgent.AsientosContables;
using PaqAgent.Auth;
using PaqAgent.Clientes;
using PaqAgent.Comprobantes;
using PaqAgent.Diagnostics;
using PaqAgent.Database;
using PaqAgent.Informes;
using PaqAgent.Menu;
using PaqAgent.MovimientosTesoreria;
using PaqAgent.Options;
using PaqAgent.OrdenesCompra;
using PaqAgent.Parametros;
using PaqAgent.Partes;
using PaqAgent.Pedidos;
using PaqAgent.PedidosVenta;
using PaqAgent.Robinet;
using PaqAgent.Saldos;
using PaqAgent.Seguridad;
using PaqAgent.Stock;
using PaqAgent.Tango;
using PaqContracts;

namespace PaqAgent;

public sealed class AgentGatewayConnector : BackgroundService
{
    private static readonly TimeSpan[] reconnectDelays =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(20),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

    private readonly AgentOptions agentOptions;
    private readonly ILogger<AgentGatewayConnector> logger;
    private readonly TimeProvider timeProvider;
    private readonly DiagnosticsRunner diagnosticsRunner;
    private readonly ISqlMigrationRunner sqlMigrationRunner;
    private readonly AuthLoginRunner authLoginRunner;
    private readonly AuthChangePasswordRunner authChangePasswordRunner;
    private readonly MenuAuthorizedRunner menuAuthorizedRunner;
    private readonly ClientesBuscarRunner clientesBuscarRunner;
    private readonly ClientesObtenerRunner clientesObtenerRunner;
    private readonly ArticulosBuscarRunner articulosBuscarRunner;
    private readonly ArticulosObtenerRunner articulosObtenerRunner;
    private readonly StockConsultarRunner stockConsultarRunner;
    private readonly SaldosConsultarRunner saldosConsultarRunner;
    private readonly PedidosPendientesRunner pedidosPendientesRunner;
    private readonly ComprobantesRecientesRunner comprobantesRecientesRunner;
    private readonly TangoVersionRunner tangoVersionRunner;
    private readonly InformesGatewayRunner informesGatewayRunner;
    private readonly AcopiosGatewayRunner acopiosGatewayRunner;
    private readonly PartesGatewayRunner partesGatewayRunner;
    private readonly ParametrosGatewayRunner parametrosGatewayRunner;
    private readonly OrdenesTrabajoCreateRunner ordenesTrabajoCreateRunner;
    private readonly OrdenesTrabajoUpdateRunner ordenesTrabajoUpdateRunner;
    private readonly OrdenesTrabajoDeleteRunner ordenesTrabajoDeleteRunner;
    private readonly OrdenesTrabajoPatchEstadoRunner ordenesTrabajoPatchEstadoRunner;
    private readonly OrdenesTrabajoCambioMasivoEstadoRunner ordenesTrabajoCambioMasivoEstadoRunner;
    private readonly AsignacionesCreateRunner asignacionesCreateRunner;
    private readonly AsignacionesUpdateRunner asignacionesUpdateRunner;
    private readonly AsignacionesPublicarRunner asignacionesPublicarRunner;
    private readonly AsignacionesCerrarRunner asignacionesCerrarRunner;
    private readonly AsignacionesCancelarRunner asignacionesCancelarRunner;
    private readonly AsignacionesItemsCreateRunner asignacionesItemsCreateRunner;
    private readonly AsignacionesItemsUpdateRunner asignacionesItemsUpdateRunner;
    private readonly AsignacionesItemsDeleteRunner asignacionesItemsDeleteRunner;
    private readonly AsignacionesOperariosPlanListRunner asignacionesOperariosPlanListRunner;
    private readonly AsignacionesItemsOperariosCreateRunner asignacionesItemsOperariosCreateRunner;
    private readonly AsignacionesItemsOperariosUpdateRunner asignacionesItemsOperariosUpdateRunner;
    private readonly AsignacionesItemsOperariosDeleteRunner asignacionesItemsOperariosDeleteRunner;
    private readonly PartesOperarioCreateRunner partesOperarioCreateRunner;
    private readonly PartesOperarioUpdateRunner partesOperarioUpdateRunner;
    private readonly PartesOperarioEnviarRunner partesOperarioEnviarRunner;
    private readonly PartesOperarioAprobarRunner partesOperarioAprobarRunner;
    private readonly PartesOperarioDevolverRunner partesOperarioDevolverRunner;
    private readonly PartesOperarioCerrarRunner partesOperarioCerrarRunner;
    private readonly PartesEntradasCreateRunner partesEntradasCreateRunner;
    private readonly PartesEntradasUpdateRunner partesEntradasUpdateRunner;
    private readonly PartesEntradasDeleteRunner partesEntradasDeleteRunner;
    private readonly PartesEntradasReclasificarRunner partesEntradasReclasificarRunner;
    private readonly PedidosVentaGatewayRunner pedidosVentaGatewayRunner;
    private readonly PedidosVentaCreateRunner pedidosVentaCreateRunner;
    private readonly PedidosVentaUpdateRunner pedidosVentaUpdateRunner;
    private readonly PedidosVentaDeleteRunner pedidosVentaDeleteRunner;
    private readonly OrdenesCompraGatewayRunner ordenesCompraGatewayRunner;
    private readonly OrdenesCompraCreateRunner ordenesCompraCreateRunner;
    private readonly OrdenesCompraUpdateRunner ordenesCompraUpdateRunner;
    private readonly OrdenesCompraDeleteRunner ordenesCompraDeleteRunner;
    private readonly MovimientosTesoreriaGatewayRunner movimientosTesoreriaGatewayRunner;
    private readonly MovimientosTesoreriaCreateRunner movimientosTesoreriaCreateRunner;
    private readonly MovimientosTesoreriaReversionRunner movimientosTesoreriaReversionRunner;
    private readonly AsientosContablesGatewayRunner asientosContablesGatewayRunner;
    private readonly AsientosContablesCreateRunner asientosContablesCreateRunner;
    private readonly AsientosContablesUpdateRunner asientosContablesUpdateRunner;
    private readonly AsientosContablesDeleteRunner asientosContablesDeleteRunner;
    private readonly SeguridadGatewayRunner seguridadGatewayRunner;
    private HubConnection? hubConnection;
    private string readiness = "network_ok";
    private readonly string agentVersion;
    private readonly string machineName;

    public AgentGatewayConnector(
        IOptions<AgentOptions> agentOptions,
        ILogger<AgentGatewayConnector> logger,
        TimeProvider timeProvider,
        DiagnosticsRunner diagnosticsRunner,
        ISqlMigrationRunner sqlMigrationRunner,
        AuthLoginRunner authLoginRunner,
        AuthChangePasswordRunner authChangePasswordRunner,
        MenuAuthorizedRunner menuAuthorizedRunner,
        ClientesBuscarRunner clientesBuscarRunner,
        ClientesObtenerRunner clientesObtenerRunner,
        ArticulosBuscarRunner articulosBuscarRunner,
        ArticulosObtenerRunner articulosObtenerRunner,
        StockConsultarRunner stockConsultarRunner,
        SaldosConsultarRunner saldosConsultarRunner,
        PedidosPendientesRunner pedidosPendientesRunner,
        ComprobantesRecientesRunner comprobantesRecientesRunner,
        TangoVersionRunner tangoVersionRunner,
        InformesGatewayRunner informesGatewayRunner,
        AcopiosGatewayRunner acopiosGatewayRunner,
        PartesGatewayRunner partesGatewayRunner,
        ParametrosGatewayRunner parametrosGatewayRunner,
        OrdenesTrabajoCreateRunner ordenesTrabajoCreateRunner,
        OrdenesTrabajoUpdateRunner ordenesTrabajoUpdateRunner,
        OrdenesTrabajoDeleteRunner ordenesTrabajoDeleteRunner,
        OrdenesTrabajoPatchEstadoRunner ordenesTrabajoPatchEstadoRunner,
        OrdenesTrabajoCambioMasivoEstadoRunner ordenesTrabajoCambioMasivoEstadoRunner,
        AsignacionesCreateRunner asignacionesCreateRunner,
        AsignacionesUpdateRunner asignacionesUpdateRunner,
        AsignacionesPublicarRunner asignacionesPublicarRunner,
        AsignacionesCerrarRunner asignacionesCerrarRunner,
        AsignacionesCancelarRunner asignacionesCancelarRunner,
        AsignacionesItemsCreateRunner asignacionesItemsCreateRunner,
        AsignacionesItemsUpdateRunner asignacionesItemsUpdateRunner,
        AsignacionesItemsDeleteRunner asignacionesItemsDeleteRunner,
        AsignacionesOperariosPlanListRunner asignacionesOperariosPlanListRunner,
        AsignacionesItemsOperariosCreateRunner asignacionesItemsOperariosCreateRunner,
        AsignacionesItemsOperariosUpdateRunner asignacionesItemsOperariosUpdateRunner,
        AsignacionesItemsOperariosDeleteRunner asignacionesItemsOperariosDeleteRunner,
        PartesOperarioCreateRunner partesOperarioCreateRunner,
        PartesOperarioUpdateRunner partesOperarioUpdateRunner,
        PartesOperarioEnviarRunner partesOperarioEnviarRunner,
        PartesOperarioAprobarRunner partesOperarioAprobarRunner,
        PartesOperarioDevolverRunner partesOperarioDevolverRunner,
        PartesOperarioCerrarRunner partesOperarioCerrarRunner,
        PartesEntradasCreateRunner partesEntradasCreateRunner,
        PartesEntradasUpdateRunner partesEntradasUpdateRunner,
        PartesEntradasDeleteRunner partesEntradasDeleteRunner,
        PartesEntradasReclasificarRunner partesEntradasReclasificarRunner,
        PedidosVentaGatewayRunner pedidosVentaGatewayRunner,
        PedidosVentaCreateRunner pedidosVentaCreateRunner,
        PedidosVentaUpdateRunner pedidosVentaUpdateRunner,
        PedidosVentaDeleteRunner pedidosVentaDeleteRunner,
        OrdenesCompraGatewayRunner ordenesCompraGatewayRunner,
        OrdenesCompraCreateRunner ordenesCompraCreateRunner,
        OrdenesCompraUpdateRunner ordenesCompraUpdateRunner,
        OrdenesCompraDeleteRunner ordenesCompraDeleteRunner,
        MovimientosTesoreriaGatewayRunner movimientosTesoreriaGatewayRunner,
        MovimientosTesoreriaCreateRunner movimientosTesoreriaCreateRunner,
        MovimientosTesoreriaReversionRunner movimientosTesoreriaReversionRunner,
        AsientosContablesGatewayRunner asientosContablesGatewayRunner,
        AsientosContablesCreateRunner asientosContablesCreateRunner,
        AsientosContablesUpdateRunner asientosContablesUpdateRunner,
        AsientosContablesDeleteRunner asientosContablesDeleteRunner,
        SeguridadGatewayRunner seguridadGatewayRunner)
    {
        this.agentOptions = agentOptions.Value;
        this.logger = logger;
        this.timeProvider = timeProvider;
        this.diagnosticsRunner = diagnosticsRunner;
        this.sqlMigrationRunner = sqlMigrationRunner;
        this.authLoginRunner = authLoginRunner;
        this.authChangePasswordRunner = authChangePasswordRunner;
        this.menuAuthorizedRunner = menuAuthorizedRunner;
        this.clientesBuscarRunner = clientesBuscarRunner;
        this.clientesObtenerRunner = clientesObtenerRunner;
        this.articulosBuscarRunner = articulosBuscarRunner;
        this.articulosObtenerRunner = articulosObtenerRunner;
        this.stockConsultarRunner = stockConsultarRunner;
        this.saldosConsultarRunner = saldosConsultarRunner;
        this.pedidosPendientesRunner = pedidosPendientesRunner;
        this.comprobantesRecientesRunner = comprobantesRecientesRunner;
        this.tangoVersionRunner = tangoVersionRunner;
        this.informesGatewayRunner = informesGatewayRunner;
        this.acopiosGatewayRunner = acopiosGatewayRunner;
        this.partesGatewayRunner = partesGatewayRunner;
        this.parametrosGatewayRunner = parametrosGatewayRunner;
        this.ordenesTrabajoCreateRunner = ordenesTrabajoCreateRunner;
        this.ordenesTrabajoUpdateRunner = ordenesTrabajoUpdateRunner;
        this.ordenesTrabajoDeleteRunner = ordenesTrabajoDeleteRunner;
        this.ordenesTrabajoPatchEstadoRunner = ordenesTrabajoPatchEstadoRunner;
        this.ordenesTrabajoCambioMasivoEstadoRunner = ordenesTrabajoCambioMasivoEstadoRunner;
        this.asignacionesCreateRunner = asignacionesCreateRunner;
        this.asignacionesUpdateRunner = asignacionesUpdateRunner;
        this.asignacionesPublicarRunner = asignacionesPublicarRunner;
        this.asignacionesCerrarRunner = asignacionesCerrarRunner;
        this.asignacionesCancelarRunner = asignacionesCancelarRunner;
        this.asignacionesItemsCreateRunner = asignacionesItemsCreateRunner;
        this.asignacionesItemsUpdateRunner = asignacionesItemsUpdateRunner;
        this.asignacionesItemsDeleteRunner = asignacionesItemsDeleteRunner;
        this.asignacionesOperariosPlanListRunner = asignacionesOperariosPlanListRunner;
        this.asignacionesItemsOperariosCreateRunner = asignacionesItemsOperariosCreateRunner;
        this.asignacionesItemsOperariosUpdateRunner = asignacionesItemsOperariosUpdateRunner;
        this.asignacionesItemsOperariosDeleteRunner = asignacionesItemsOperariosDeleteRunner;
        this.partesOperarioCreateRunner = partesOperarioCreateRunner;
        this.partesOperarioUpdateRunner = partesOperarioUpdateRunner;
        this.partesOperarioEnviarRunner = partesOperarioEnviarRunner;
        this.partesOperarioAprobarRunner = partesOperarioAprobarRunner;
        this.partesOperarioDevolverRunner = partesOperarioDevolverRunner;
        this.partesOperarioCerrarRunner = partesOperarioCerrarRunner;
        this.partesEntradasCreateRunner = partesEntradasCreateRunner;
        this.partesEntradasUpdateRunner = partesEntradasUpdateRunner;
        this.partesEntradasDeleteRunner = partesEntradasDeleteRunner;
        this.partesEntradasReclasificarRunner = partesEntradasReclasificarRunner;
        this.pedidosVentaGatewayRunner = pedidosVentaGatewayRunner;
        this.pedidosVentaCreateRunner = pedidosVentaCreateRunner;
        this.pedidosVentaUpdateRunner = pedidosVentaUpdateRunner;
        this.pedidosVentaDeleteRunner = pedidosVentaDeleteRunner;
        this.ordenesCompraGatewayRunner = ordenesCompraGatewayRunner;
        this.ordenesCompraCreateRunner = ordenesCompraCreateRunner;
        this.ordenesCompraUpdateRunner = ordenesCompraUpdateRunner;
        this.ordenesCompraDeleteRunner = ordenesCompraDeleteRunner;
        this.movimientosTesoreriaGatewayRunner = movimientosTesoreriaGatewayRunner;
        this.movimientosTesoreriaCreateRunner = movimientosTesoreriaCreateRunner;
        this.movimientosTesoreriaReversionRunner = movimientosTesoreriaReversionRunner;
        this.asientosContablesGatewayRunner = asientosContablesGatewayRunner;
        this.asientosContablesCreateRunner = asientosContablesCreateRunner;
        this.asientosContablesUpdateRunner = asientosContablesUpdateRunner;
        this.asientosContablesDeleteRunner = asientosContablesDeleteRunner;
        this.seguridadGatewayRunner = seguridadGatewayRunner;
        agentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        machineName = Environment.MachineName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!agentOptions.HasRequiredIdentity)
        {
            logger.LogError(
                "PaqAgent no inicia conexión: faltan agentId, clientId, agentToken o gatewayUrl. Completar appsettings.local.json.");
            return;
        }

        if (string.Equals(agentOptions.AgentToken, "dev-agent-token", StringComparison.Ordinal))
        {
            logger.LogError("PaqAgent rechaza AgentToken prohibido (dev-agent-token). Usar token real de alta.");
            return;
        }

        await sqlMigrationRunner.RunAsync(agentOptions, stoppingToken).ConfigureAwait(false);

        var sqlServerName = agentOptions.HasSqlConfig ? agentOptions.Sql.Server : null;

        logger.LogInformation(
            "PaqAgent starting machineName={MachineName} sqlServerName={SqlServerName} agentVersion={AgentVersion} agentId={AgentId} hub={Hub}",
            machineName,
            sqlServerName ?? "(none)",
            agentVersion,
            agentOptions.AgentId,
            HubUrlBuilder.BuildSafeHubUrlForLogs(
                agentOptions.GatewayUrl,
                agentOptions.AgentId,
                agentOptions.ClientId));

        readiness = "gateway_authenticated";

        var hubUrl = HubUrlBuilder.BuildHubUrl(
            agentOptions.GatewayUrl,
            agentOptions.AgentId,
            agentOptions.ClientId,
            agentOptions.AgentToken);

        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect(reconnectDelays)
            .Build();

        hubConnection.On<JobRequest>(HubMethodNames.ExecuteJob, async request =>
        {
            await HandleExecuteJobAsync(request).ConfigureAwait(false);
        });

        hubConnection.Reconnecting += error =>
        {
            readiness = "network_ok";
            logger.LogWarning(error, "Reconectando al Gateway");
            return Task.CompletedTask;
        };

        hubConnection.Reconnected += connectionId =>
        {
            readiness = "gateway_authenticated";
            logger.LogInformation("Reconectado al Gateway connectionId={ConnectionId}", connectionId);
            return Task.CompletedTask;
        };

        hubConnection.Closed += error =>
        {
            readiness = "network_ok";
            if (error is null)
            {
                logger.LogWarning("Conexión al Gateway cerrada");
            }
            else
            {
                logger.LogError(error, "Conexión al Gateway cerrada con error (token inválido u otro). El agente no queda online.");
            }

            return Task.CompletedTask;
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (hubConnection.State == HubConnectionState.Disconnected)
                {
                    logger.LogInformation("Conectando al Gateway…");
                    await hubConnection.StartAsync(stoppingToken).ConfigureAwait(false);
                    if (readiness is "network_ok")
                    {
                        readiness = "gateway_authenticated";
                    }

                    logger.LogInformation(
                        "Conectado al Gateway. readiness={Readiness} agentId={AgentId}",
                        readiness,
                        agentOptions.AgentId);
                }

                await SendHeartbeatAsync(stoppingToken).ConfigureAwait(false);
                await Task.Delay(
                    TimeSpan.FromSeconds(AgentDefaults.HeartbeatSeconds),
                    stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                readiness = "network_ok";
                logger.LogError(
                    ex,
                    "Fallo de conexión/heartbeat al Gateway. Reintento con backoff. agentId={AgentId}",
                    agentOptions.AgentId);

                if (hubConnection.State != HubConnectionState.Disconnected)
                {
                    try
                    {
                        await hubConnection.StopAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                try
                {
                    await Task.Delay(reconnectDelays[0], stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        if (hubConnection is null || hubConnection.State != HubConnectionState.Connected)
        {
            return;
        }

        await hubConnection.InvokeAsync(
            HubMethodNames.Heartbeat,
            new AgentHeartbeat
            {
                AgentId = agentOptions.AgentId,
                ClientId = agentOptions.ClientId,
                AgentVersion = agentVersion,
                Readiness = readiness,
                TimestampUtc = timeProvider.GetUtcNow()
            },
            cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Heartbeat OK readiness={Readiness}", readiness);
    }

    private async Task HandleExecuteJobAsync(JobRequest request)
    {
        if (hubConnection is null)
        {
            return;
        }

        logger.LogInformation(
            "ExecuteJob recibido operation={Operation} jobId={JobId} traceId={TraceId}",
            request.Operation,
            request.JobId,
            request.TraceId);

        var started = timeProvider.GetUtcNow();
        JobResult result;

        if (string.Equals(request.Operation, JobOperations.DiagnosticsRun, StringComparison.Ordinal))
        {
            var outcome = await diagnosticsRunner
                .RunAsync(agentOptions, agentVersion, machineName, CancellationToken.None)
                .ConfigureAwait(false);
            readiness = outcome.Readiness;
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.AuthLogin, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando auth.login jobId={JobId} traceId={TraceId} (password_hash omitido de logs)",
                request.JobId,
                request.TraceId);
            var outcome = await authLoginRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.AuthChangePassword, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando auth.changePassword jobId={JobId} traceId={TraceId} (password_hash omitido de logs)",
                request.JobId,
                request.TraceId);
            var changeOutcome = await authChangePasswordRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = changeOutcome.Status,
                Data = changeOutcome.Data,
                ErrorCode = changeOutcome.ErrorCode,
                ErrorMessage = changeOutcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.MenuAuthorized, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando menu.authorized jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await menuAuthorizedRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ClientesBuscar, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando clientes.buscar jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await clientesBuscarRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ClientesObtener, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando clientes.obtener jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await clientesObtenerRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ArticulosBuscar, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando articulos.buscar jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await articulosBuscarRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ArticulosObtener, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando articulos.obtener jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await articulosObtenerRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.StockConsultar, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando stock.consultar jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await stockConsultarRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.SaldosConsultar, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando saldos.consultar jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await saldosConsultarRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.PedidosPendientes, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando pedidos.pendientes jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await pedidosPendientesRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ComprobantesRecientes, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando comprobantes.recientes jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await comprobantesRecientesRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.TangoVersion, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando tango.version jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await tangoVersionRunner
                .RunAsync(request.Parameters, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (InformesCatalog.TryGet(request.Operation, out var informesDefinition)
                 || RobinetCatalog.TryGet(request.Operation, out informesDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            var outcome = await informesGatewayRunner
                .RunAsync(
                    informesDefinition,
                    agentOptions,
                    request.Parameters,
                    request.TimeoutSeconds,
                    CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (AcopiosCatalog.TryGet(request.Operation, out var acopiosDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            var outcome = await acopiosGatewayRunner
                .RunAsync(
                    acopiosDefinition,
                    agentOptions,
                    request.Parameters,
                    request.TimeoutSeconds,
                    CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (string.Equals(request.Operation, JobOperations.ParametrosUpdate, StringComparison.Ordinal))
        {
            logger.LogInformation(
                "Ejecutando parametros.update jobId={JobId} traceId={TraceId}",
                request.JobId,
                request.TraceId);
            var outcome = await parametrosGatewayRunner
                .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (PartesCatalog.TryGet(request.Operation, out var partesDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            PartesOutcome outcome;
            if (partesDefinition.Shape == PartesResponseShape.OrdenTrabajoCreate)
            {
                outcome = await ordenesTrabajoCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.OrdenTrabajoUpdate)
            {
                outcome = await ordenesTrabajoUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.OrdenTrabajoDelete)
            {
                outcome = await ordenesTrabajoDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.OrdenTrabajoPatchEstado)
            {
                outcome = await ordenesTrabajoPatchEstadoRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.OrdenTrabajoCambioMasivoEstado)
            {
                outcome = await ordenesTrabajoCambioMasivoEstadoRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionCreate)
            {
                outcome = await asignacionesCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionUpdate)
            {
                outcome = await asignacionesUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionPublicar)
            {
                outcome = await asignacionesPublicarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionCerrar)
            {
                outcome = await asignacionesCerrarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionCancelar)
            {
                outcome = await asignacionesCancelarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemCreate)
            {
                outcome = await asignacionesItemsCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemUpdate)
            {
                outcome = await asignacionesItemsUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemDelete)
            {
                outcome = await asignacionesItemsDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionOperariosPlanList)
            {
                outcome = await asignacionesOperariosPlanListRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemOperariosCreate)
            {
                outcome = await asignacionesItemsOperariosCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemOperariosUpdate)
            {
                outcome = await asignacionesItemsOperariosUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.AsignacionItemOperariosDelete)
            {
                outcome = await asignacionesItemsOperariosDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioCreate)
            {
                outcome = await partesOperarioCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioUpdate)
            {
                outcome = await partesOperarioUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioEnviar)
            {
                outcome = await partesOperarioEnviarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioAprobar)
            {
                outcome = await partesOperarioAprobarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioDevolver)
            {
                outcome = await partesOperarioDevolverRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioCerrar)
            {
                outcome = await partesOperarioCerrarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioEntradasCreate)
            {
                outcome = await partesEntradasCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioEntradasUpdate)
            {
                outcome = await partesEntradasUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioEntradasDelete)
            {
                outcome = await partesEntradasDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (partesDefinition.Shape == PartesResponseShape.ParteOperarioEntradasReclasificar)
            {
                outcome = await partesEntradasReclasificarRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                outcome = await partesGatewayRunner
                    .RunAsync(
                        partesDefinition,
                        agentOptions,
                        request.Parameters,
                        request.TimeoutSeconds,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (PedidosVentaCatalog.TryGet(request.Operation, out var pedidosVentaDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            PedidosVentaOutcome outcome;
            if (pedidosVentaDefinition.Shape == PedidosVentaResponseShape.Create)
            {
                outcome = await pedidosVentaCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (pedidosVentaDefinition.Shape == PedidosVentaResponseShape.Update)
            {
                outcome = await pedidosVentaUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (pedidosVentaDefinition.Shape == PedidosVentaResponseShape.Delete)
            {
                outcome = await pedidosVentaDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                outcome = await pedidosVentaGatewayRunner
                    .RunAsync(
                        pedidosVentaDefinition,
                        agentOptions,
                        request.Parameters,
                        request.TimeoutSeconds,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (OrdenesCompraCatalog.TryGet(request.Operation, out var ordenesCompraDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            OrdenesCompraOutcome outcome;
            if (ordenesCompraDefinition.Shape == OrdenesCompraResponseShape.Create)
            {
                outcome = await ordenesCompraCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (ordenesCompraDefinition.Shape == OrdenesCompraResponseShape.Update)
            {
                outcome = await ordenesCompraUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (ordenesCompraDefinition.Shape == OrdenesCompraResponseShape.Delete)
            {
                outcome = await ordenesCompraDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                outcome = await ordenesCompraGatewayRunner
                    .RunAsync(
                        ordenesCompraDefinition,
                        agentOptions,
                        request.Parameters,
                        request.TimeoutSeconds,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (MovimientosTesoreriaCatalog.TryGet(request.Operation, out var movimientosTesoreriaDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            MovimientosTesoreriaOutcome outcome;
            if (movimientosTesoreriaDefinition.Shape == MovimientosTesoreriaResponseShape.Create)
            {
                outcome = await movimientosTesoreriaCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (movimientosTesoreriaDefinition.Shape == MovimientosTesoreriaResponseShape.Reversion)
            {
                outcome = await movimientosTesoreriaReversionRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                outcome = await movimientosTesoreriaGatewayRunner
                    .RunAsync(
                        movimientosTesoreriaDefinition,
                        agentOptions,
                        request.Parameters,
                        request.TimeoutSeconds,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (AsientosContablesCatalog.TryGet(request.Operation, out var asientosContablesDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            AsientosContablesOutcome outcome;
            if (asientosContablesDefinition.Shape == AsientosContablesResponseShape.Create)
            {
                outcome = await asientosContablesCreateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (asientosContablesDefinition.Shape == AsientosContablesResponseShape.Update)
            {
                outcome = await asientosContablesUpdateRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else if (asientosContablesDefinition.Shape == AsientosContablesResponseShape.Delete)
            {
                outcome = await asientosContablesDeleteRunner
                    .RunAsync(agentOptions, request.Parameters, request.TimeoutSeconds, CancellationToken.None)
                    .ConfigureAwait(false);
            }
            else
            {
                outcome = await asientosContablesGatewayRunner
                    .RunAsync(
                        asientosContablesDefinition,
                        agentOptions,
                        request.Parameters,
                        request.TimeoutSeconds,
                        CancellationToken.None)
                    .ConfigureAwait(false);
            }

            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (SeguridadCatalog.TryGet(request.Operation, out var seguridadDefinition))
        {
            logger.LogInformation(
                "Ejecutando {Operation} jobId={JobId} traceId={TraceId}",
                request.Operation,
                request.JobId,
                request.TraceId);
            var outcome = await seguridadGatewayRunner
                .RunAsync(
                    seguridadDefinition,
                    agentOptions,
                    request.Parameters,
                    request.TimeoutSeconds,
                    CancellationToken.None)
                .ConfigureAwait(false);
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = outcome.Status,
                Data = outcome.Data,
                ErrorCode = outcome.ErrorCode,
                ErrorMessage = outcome.ErrorMessage,
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else if (readiness is "network_ok")
        {
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = JobStatuses.Degraded,
                ErrorCode = "AGENT_NOT_READY",
                ErrorMessage = $"readiness={readiness}",
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }
        else
        {
            result = new JobResult
            {
                TraceId = request.TraceId,
                JobId = request.JobId,
                Status = JobStatuses.Failed,
                ErrorCode = "OPERATION_NOT_ALLOWED",
                ErrorMessage =
                    $"operation '{request.Operation}' not in whitelist (shell + D1–D5 + D6 + Seguridad admin + AsientosContables)",
                DurationMs = (long)(timeProvider.GetUtcNow() - started).TotalMilliseconds
            };
        }

        await hubConnection.InvokeAsync(HubMethodNames.CompleteJob, result).ConfigureAwait(false);
        logger.LogInformation(
            "CompleteJob enviado status={Status} jobId={JobId} traceId={TraceId} durationMs={DurationMs} readiness={Readiness}",
            result.Status,
            result.JobId,
            result.TraceId,
            result.DurationMs,
            readiness);
    }
}
