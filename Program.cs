using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;
using TransparenciaBot.Servicos;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(
    "TransparenciaBotDb")
    ?? throw new InvalidOperationException(
        "A string de conexão 'TransparenciaBotDb' não foi configurada.");

builder.Services.AddControllers();

builder.Services.AddDbContext<TransparenciaBotDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRegistroMensagemServico, RegistroMensagemServico>();
builder.Services.AddScoped<ConsultaGastosBancoServico>();
builder.Services.AddScoped<FormatadorRespostaGastosServico>();
builder.Services.AddScoped<InterpretadorConsultaServico>();
builder.Services.AddScoped<RespostaConsultaServico>();

builder.Services.AddHttpClient<ConsultaCamaraServico>(cliente =>
{
    cliente.BaseAddress = new Uri("https://dadosabertos.camara.leg.br/");
});

builder.Services.AddHttpClient<InterpretadorConsultaOpenAI>();

builder.Services.AddHttpClient<ImportadorGastosCamaraServico>();

builder.Services.AddHttpClient<EnvioWhatsAppServico>(cliente =>
{
    cliente.BaseAddress = new Uri("https://graph.facebook.com/v25.0/");
});

var app = builder.Build();

if (args.Length == 2 &&
    int.TryParse(args[1], out var anoParaImportar) &&
    (args[0].Equals(
        "--importar-gastos",
        StringComparison.OrdinalIgnoreCase) ||
     args[0].Equals(
        "--reimportar-gastos",
        StringComparison.OrdinalIgnoreCase)))
{
    using var escopo = app.Services.CreateScope();

    var importador = escopo.ServiceProvider
        .GetRequiredService<ImportadorGastosCamaraServico>();

    var reimportar = args[0].Equals(
        "--reimportar-gastos",
        StringComparison.OrdinalIgnoreCase);

    var quantidade = reimportar
        ? await importador.ReimportarAnoAsync(
            anoParaImportar,
            CancellationToken.None)
        : await importador.ImportarAnoAsync(
            anoParaImportar,
            CancellationToken.None);

    Console.WriteLine(
        $"{quantidade} gastos de {anoParaImportar} foram importados.");

    return;
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();