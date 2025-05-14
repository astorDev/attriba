using System.Text.Json;
using Attriba;
using Microsoft.EntityFrameworkCore;
using Persic;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

builder.Services.AddPostgres<Db>();

var app = builder.Build();

await app.Services.EnsureRecreated<Db>();

app.MapPut("/resources/{id}", async (string id, ResourceCandidate candidate, Db db) => {
    var record = Record.From(candidate, id);
    
    var saved = await db.Records.Put(record, updateOverwrite: null);
    await db.SaveChangesAsync();

    return saved.ToResource();
});

app.MapPatch("/resources/{id}", async (string id, ResourceCandidate candidate, Db db) => {
    var record = await db.Records.Search(id) ?? throw new KeyNotFoundException($"Resource with id `{id}` not found");
    
    record.Labels = Merge.Of(record.Labels, candidate.Labels).AsJsonDocument();
    
    await db.SaveChangesAsync();

    return record.ToResource();
});

app.MapGet("/resources/{id}", async (string id, Db db) => {
    var record = await db.Records.Search(id) ?? throw new KeyNotFoundException($"Resource with id `{id}` not found");
    return record.ToResource();
});

app.Run();

public record ResourceCandidate(
    Dictionary<string, string>? Labels
);

public record Resource(
    string Id,
    Dictionary<string, string> Labels
);

public class Record : IDbEntity<string> {
    public string Id { get; set; } = null!;
    public required JsonDocument Labels { get; set; }

    public static Record From(ResourceCandidate candidate, string id) => new()
    {
        Id = id,
        Labels = Json.DocumentFrom(candidate.Labels)
    };

    public Resource ToResource() => new(
        Id,
        Labels.ToDictionary()
    );
}

public class Db(DbContextOptions<Db> options) : DbContext(options) {
    public DbSet<Record> Records { get; set; } = null!;
}