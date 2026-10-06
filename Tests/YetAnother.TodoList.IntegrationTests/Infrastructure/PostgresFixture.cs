using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using YetAnother.TodoList.Infrastructure;

namespace YetAnother.TodoList.IntegrationTests.Infrastructure;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private DbContextOptions<TodoDbContext> _options = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public TodoDbContext CreateContext() => new(_options);

    public async Task ResetAsync()
    {
        await using var context = CreateContext();
        await context.Todos.ExecuteDeleteAsync();
    }
}

[CollectionDefinition(Name)]
public class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Postgres";
}
