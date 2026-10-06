using FluentAssertions;
using YetAnother.TodoList.Domain;
using YetAnother.TodoList.Infrastructure;

namespace YetAnother.TodoList.IntegrationTests.Infrastructure;

[Collection(PostgresCollection.Name)]
public class TodoItemRepositoryTests(PostgresFixture fixture) : IAsyncLifetime
{
    public Task InitializeAsync() => fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<TodoItem> SeedAsync(string description)
    {
        await using var context = fixture.CreateContext();
        var item = new TodoItem { Description = description };
        context.Todos.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    private async Task<List<TodoItem>> ReadAllAsync()
    {
        await using var context = fixture.CreateContext();
        return context.Todos.OrderBy(t => t.Id).ToList();
    }

    [Fact]
    public async Task CreateItem_PersistsItemWithGeneratedId()
    {
        // Arrange
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        var created = await repository.CreateItem("Buy milk");

        // Assert
        created.Id.Should().BeGreaterThan(0);
        var stored = await ReadAllAsync();
        stored.Should().ContainSingle()
              .Which.Should().BeEquivalentTo(new { created.Id, Description = "Buy milk" });
    }

    [Fact]
    public async Task GetAllItemsAsync_EmptyTable_ReturnsEmptyList()
    {
        // Arrange
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        var items = await repository.GetAllItemsAsync();

        // Assert
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllItemsAsync_ReturnsAllSeededItems()
    {
        // Arrange
        await SeedAsync("first");
        await SeedAsync("second");
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        var items = await repository.GetAllItemsAsync();

        // Assert
        items.Select(i => i.Description).Should().BeEquivalentTo("first", "second");
    }

    [Fact]
    public async Task UpdateItem_ExistingItem_PersistsNewDescription()
    {
        // Arrange
        var seeded = await SeedAsync("old");
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        var updated = await repository.UpdateItem(new TodoItem { Id = seeded.Id, Description = "new" });

        // Assert
        updated.Description.Should().Be("new");
        (await ReadAllAsync()).Should().ContainSingle()
            .Which.Description.Should().Be("new");
    }

    [Fact]
    public async Task UpdateItem_UnknownId_ReturnsNullAndLeavesDataUnchanged()
    {
        // Arrange
        await SeedAsync("untouched");
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        var updated = await repository.UpdateItem(new TodoItem { Id = 99999, Description = "x" });

        // Assert
        updated.Should().BeNull();
        (await ReadAllAsync()).Should().ContainSingle()
            .Which.Description.Should().Be("untouched");
    }

    [Fact]
    public async Task DeleteItem_ExistingItem_RemovesOnlyThatItem()
    {
        // Arrange
        var toDelete = await SeedAsync("delete me");
        await SeedAsync("keep me");
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        await repository.DeleteItem(toDelete.Id);

        // Assert
        (await ReadAllAsync()).Should().ContainSingle()
            .Which.Description.Should().Be("keep me");
    }

    [Fact]
    public async Task DeleteItem_UnknownId_IsNoOp()
    {
        // Arrange
        await SeedAsync("keep me");
        await using var context = fixture.CreateContext();
        var repository = new TodoItemRepository(context);

        // Act
        await repository.DeleteItem(99999);

        // Assert
        (await ReadAllAsync()).Should().ContainSingle();
    }
}
