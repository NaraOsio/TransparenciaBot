using Microsoft.EntityFrameworkCore;
using TransparenciaBot.Dados;
using TransparenciaBot.Servicos;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TransparenciaBotDb")
    ?? throw new InvalidOperationException(
        "A string de conexão 'TransparenciaBotDb' não foi configurada.");

builder.Services.AddControllers();
builder.Services.AddDbContext<TransparenciaBotDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IRegistroMensagemServico, RegistroMensagemServico>();
builder.Services.AddScoped<ConsultaGastosBancoServico>();
builder.Services.AddScoped<FormatadorRespostaGastosServico>();
builder.Services.AddHttpClient<ConsultaCamaraServico>(cliente =>
{
    cliente.BaseAddress = new Uri("https://dadosabertos.camara.leg.br/");
});
builder.Services.AddHttpClient<InterpretadorConsultaOpenAI>();
builder.Services.AddHttpClient<ImportadorGastosCamaraServico>();

var app = builder.Build();
if (args.Length == 2 &&
    args[0].Equals("--importar-gastos",
        StringComparison.OrdinalIgnoreCase) &&
    int.TryParse(args[1], out var anoParaImportar))
{
    using var escopo = app.Services.CreateScope();

    var importador = escopo.ServiceProvider
        .GetRequiredService<ImportadorGastosCamaraServico>();

    var quantidade = await importador.ImportarAnoAsync(
        anoParaImportar,
        CancellationToken.None);

    Console.WriteLine(
        $"{quantidade} gastos de {anoParaImportar} foram importados.");

    return;
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
