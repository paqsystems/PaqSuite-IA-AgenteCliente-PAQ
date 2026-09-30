using Microsoft.Extensions.Options;
using PaqAgent;
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
using PaqAgent.Options;
using PaqAgent.Parametros;
using PaqAgent.MovimientosTesoreria;
using PaqAgent.OrdenesCompra;
using PaqAgent.Partes;
using PaqAgent.Pedidos;
using PaqAgent.PedidosVenta;
using PaqAgent.Saldos;
using PaqAgent.Seguridad;
using PaqAgent.Stock;
using PaqAgent.Tango;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

builder.Services.AddOptions<AgentOptions>()
    .Bind(builder.Configuration)
    .Validate(
        options => !string.Equals(options.AgentToken, "dev-agent-token", StringComparison.Ordinal),
        "AgentToken 'dev-agent-token' is forbidden.");

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ISqlConnectionPinger, SqlConnectionPinger>();
builder.Services.AddSingleton<ISqlExecutor, SqlExecutor>();
builder.Services.AddSingleton<SqlScriptLoader>();
builder.Services.AddSingleton<ISqlMigrationRunner, SqlMigrationRunner>();
builder.Services.AddSingleton<DiagnosticsRunner>();
builder.Services.AddSingleton<IAuthLoginSpExecutor, SqlAuthLoginSpExecutor>();
builder.Services.AddSingleton<AuthLoginRunner>();
builder.Services.AddSingleton<IAuthChangePasswordSpExecutor, SqlAuthChangePasswordSpExecutor>();
builder.Services.AddSingleton<AuthChangePasswordRunner>();
builder.Services.AddSingleton<IMenuAuthorizedSpExecutor, SqlMenuAuthorizedSpExecutor>();
builder.Services.AddSingleton<MenuAuthorizedRunner>();
builder.Services.AddSingleton<IClientesBuscarSpExecutor, SqlClientesBuscarSpExecutor>();
builder.Services.AddSingleton<ClientesBuscarRunner>();
builder.Services.AddSingleton<IClientesObtenerSpExecutor, SqlClientesObtenerSpExecutor>();
builder.Services.AddSingleton<ClientesObtenerRunner>();
builder.Services.AddSingleton<IArticulosBuscarSpExecutor, SqlArticulosBuscarSpExecutor>();
builder.Services.AddSingleton<ArticulosBuscarRunner>();
builder.Services.AddSingleton<IArticulosObtenerSpExecutor, SqlArticulosObtenerSpExecutor>();
builder.Services.AddSingleton<ArticulosObtenerRunner>();
builder.Services.AddSingleton<IStockConsultarSpExecutor, SqlStockConsultarSpExecutor>();
builder.Services.AddSingleton<StockConsultarRunner>();
builder.Services.AddSingleton<ISaldosConsultarSpExecutor, SqlSaldosConsultarSpExecutor>();
builder.Services.AddSingleton<SaldosConsultarRunner>();
builder.Services.AddSingleton<IPedidosPendientesSpExecutor, SqlPedidosPendientesSpExecutor>();
builder.Services.AddSingleton<PedidosPendientesRunner>();
builder.Services.AddSingleton<IComprobantesRecientesSpExecutor, SqlComprobantesRecientesSpExecutor>();
builder.Services.AddSingleton<ComprobantesRecientesRunner>();
builder.Services.AddSingleton<ITangoVersionRegistryReader, WindowsTangoVersionRegistryReader>();
builder.Services.AddSingleton<TangoVersionRunner>();
builder.Services.AddSingleton<IInformesSpExecutor, SqlInformesSpExecutor>();
builder.Services.AddSingleton<InformesGatewayRunner>();
builder.Services.AddSingleton<SeguridadGatewayRunner>();
builder.Services.AddSingleton<AcopiosGatewayRunner>();
builder.Services.AddSingleton<PartesGatewayRunner>();
builder.Services.AddSingleton<ParametrosGatewayRunner>();
builder.Services.AddSingleton<OrdenesTrabajoCreateRunner>();
builder.Services.AddSingleton<OrdenesTrabajoUpdateRunner>();
builder.Services.AddSingleton<OrdenesTrabajoDeleteRunner>();
builder.Services.AddSingleton<OrdenesTrabajoPatchEstadoRunner>();
builder.Services.AddSingleton<OrdenesTrabajoCambioMasivoEstadoRunner>();
builder.Services.AddSingleton<AsignacionesCreateRunner>();
builder.Services.AddSingleton<AsignacionesUpdateRunner>();
builder.Services.AddSingleton<AsignacionesPublicarRunner>();
builder.Services.AddSingleton<AsignacionesCerrarRunner>();
builder.Services.AddSingleton<AsignacionesCancelarRunner>();
builder.Services.AddSingleton<AsignacionesItemsCreateRunner>();
builder.Services.AddSingleton<AsignacionesItemsUpdateRunner>();
builder.Services.AddSingleton<AsignacionesItemsDeleteRunner>();
builder.Services.AddSingleton<AsignacionesOperariosPlanListRunner>();
builder.Services.AddSingleton<AsignacionesItemsOperariosCreateRunner>();
builder.Services.AddSingleton<AsignacionesItemsOperariosUpdateRunner>();
builder.Services.AddSingleton<AsignacionesItemsOperariosDeleteRunner>();
builder.Services.AddSingleton<PartesOperarioCreateRunner>();
builder.Services.AddSingleton<PartesOperarioUpdateRunner>();
builder.Services.AddSingleton<PartesOperarioEnviarRunner>();
builder.Services.AddSingleton<PartesOperarioAprobarRunner>();
builder.Services.AddSingleton<PartesOperarioDevolverRunner>();
builder.Services.AddSingleton<PartesOperarioCerrarRunner>();
builder.Services.AddSingleton<PartesEntradasCreateRunner>();
builder.Services.AddSingleton<PartesEntradasUpdateRunner>();
builder.Services.AddSingleton<PartesEntradasDeleteRunner>();
builder.Services.AddSingleton<PartesEntradasReclasificarRunner>();
builder.Services.AddSingleton<PedidosVentaGatewayRunner>();
builder.Services.AddSingleton<PedidosVentaCreateRunner>();
builder.Services.AddSingleton<PedidosVentaUpdateRunner>();
builder.Services.AddSingleton<PedidosVentaDeleteRunner>();
builder.Services.AddSingleton<OrdenesCompraGatewayRunner>();
builder.Services.AddSingleton<OrdenesCompraCreateRunner>();
builder.Services.AddSingleton<OrdenesCompraUpdateRunner>();
builder.Services.AddSingleton<OrdenesCompraDeleteRunner>();
builder.Services.AddSingleton<MovimientosTesoreriaGatewayRunner>();
builder.Services.AddSingleton<MovimientosTesoreriaCreateRunner>();
builder.Services.AddSingleton<MovimientosTesoreriaReversionRunner>();
builder.Services.AddSingleton<AsientosContablesGatewayRunner>();
builder.Services.AddSingleton<AsientosContablesCreateRunner>();
builder.Services.AddSingleton<AsientosContablesUpdateRunner>();
builder.Services.AddSingleton<AsientosContablesDeleteRunner>();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "PaqAgent";
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine("logs", "paqagent-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddHostedService<AgentGatewayConnector>();

try
{
    var host = builder.Build();
    Log.Information("PaqAgent host starting");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "PaqAgent host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
