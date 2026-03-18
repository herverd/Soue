using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração de porta explícita para o Kestrel
builder.WebHost.ConfigureKestrel(opt => opt.ListenAnyIP(8080));

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// GARANTIR BANCO NA INICIALIZAÇÃO
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// CONFIGURAÇÃO SWAGGER ROBUSTA
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPA API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000
});

app.MapGet("/api/cotacoes", async (AppDbContext db) =>
    await db.Cotacoes.OrderByDescending(x => x.Data).ToListAsync());

// Rota extra de Health Check para testar se a API está viva sem o Swagger
app.MapGet("/health", () => Results.Ok(new { Status = "UP", Time = DateTime.Now }));

app.Run();

// --- MODELOS (Mantenha igual) ---
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