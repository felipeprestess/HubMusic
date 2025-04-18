using GraphQL;
using GraphQL.Server;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using HubMusic.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A variável de ambiente DB_CONNECTION_STRING não está configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<SongQuery>();
builder.Services.AddScoped<SongType>();
builder.Services.AddScoped<ISchema, SongSchema>();
builder.Services.AddGraphQL(options =>
{
    options.AddSystemTextJson(); // Usa System.Text.Json para serialização
    options.AddSchema<SongSchema>(); // Configura o schema
    options.AddGraphTypes(typeof(SongSchema).Assembly); // Adiciona os tipos GraphQL

    // Configurar provedor de informações de erro
    options.AddErrorInfoProvider(opt =>
    {
        opt.ExposeExceptionDetails = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    });

    // Configuração de execuçã
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "HubMusicAPI";
    config.Title = "HubMusicAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "HubMusicAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/api/songs", async (ApplicationDbContext db) => 
    await db.Songs.ToListAsync().ConfigureAwait(false));

app.MapGet("/api/songs/{id}", async (int id, ApplicationDbContext db) => 
    await db.Songs.FindAsync(id).ConfigureAwait(false) is Song song ? Results.Ok(song) : Results.NotFound());

app.MapPost("/api/song", async (CreateSongRequest request, ApplicationDbContext db) =>
{
    var song = new Song
    {
        Title = request.Title,
        Artist = request.Artist,
        Duration = request.Duration,
        Genre = request.Genre,
        Album = request.Album ?? "Sem Album",
    };

    db.Songs.Add(song);
    await db.SaveChangesAsync().ConfigureAwait(false);
    return Results.Created($"/api/songs/{song.Id}", song);
});

app.MapPut("/api/song/{id}", async (int id, UpdateSongRequest request, ApplicationDbContext db) =>
{
    var song = await db.Songs.FindAsync(id).ConfigureAwait(false);
    if (song is null) return Results.NotFound();

    song.Title = request.Title ?? song.Title;
    song.Artist = request.Artist ?? song.Artist;
    song.Duration = request.Duration ?? song.Duration;
    song.Genre = request.Genre ?? song.Genre;

    await db.SaveChangesAsync().ConfigureAwait(false);

    return Results.NoContent();
});

app.MapDelete("/api/song/{id}", async (int id, ApplicationDbContext db) =>
{
    var song = await db.Songs.FindAsync(id).ConfigureAwait(false);
    if (song is null) return Results.NotFound();

    db.Remove(song);
    await db.SaveChangesAsync().ConfigureAwait(false);
    
    return Results.NoContent();
});

// Configura o middleware do GraphQL
app.UseGraphQL<ISchema>();
app.UseGraphQLPlayground("/ui/playground"); // Interface para testar consultas GraphQL

app.Run();
