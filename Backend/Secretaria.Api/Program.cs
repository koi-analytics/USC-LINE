using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Controllers e OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Permite que o site dos alunos na Vercel acesse a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelAlunos", policy =>
    {
        policy
            .WithOrigins("https://usc-line.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Conexão com o Supabase
var connectionString = builder.Configuration.GetConnectionString("Supabase");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "A conexão com o Supabase não foi configurada."
    );
}

builder.Services.AddSingleton<NpgsqlDataSource>(_ =>
{
    return NpgsqlDataSource.Create(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Habilita o CORS antes de atender as requisições
app.UseRouting();
app.UseCors("VercelAlunos");

app.MapControllers();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        status = "ok",
        sistema = "Secretaria UNISAGRADO",
        mensagem = "API funcionando"
    });
});

app.Run();
