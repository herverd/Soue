using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// 1. INSTRUÇÕES DE NÍVEL SUPERIOR (Setup do App)
var builder = Host.CreateApplicationBuilder(args);

// Configuração do Banco SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

// 2. DECLARAÇÕES DE TIPOS (Abaixo de todo o código acima)

public class Cotacao
{
    public int Id { get; set; }
    public string Moeda { get; set; } = "USD";
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Cotacao> Cotacoes => Set<Cotacao>();
}

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RPA Worker iniciado...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Cria o banco se não existir
                await db.Database.EnsureCreatedAsync();

                var nova = new Cotacao
                {
                    Valor = 5.0m + (decimal)Random.Shared.NextDouble(),
                    Data = DateTime.Now
                };

                db.Cotacoes.Add(nova);
                await db.SaveChangesAsync();

                _logger.LogInformation("RPA: Cotacao Salva: {V} em {D}", nova.Valor, nova.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError("RPA Erro: {M}", ex.Message);
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}