using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using YetAnother.TodoList.Api.Extensions;
using YetAnother.TodoList.Application.Dtos;
using YetAnother.TodoList.Application.Repositories;
using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.UnitTests.Api.Extensions;

public class EndpointsTests
{
    private readonly ITodoItemRepository _repository = Substitute.For<ITodoItemRepository>();

    [Fact]
    public async Task GetAllItemsAsync_ReturnsMappedDtos()
    {
        // Arrange
        _repository.GetAllItemsAsync().Returns(new List<TodoItem>
        {
            new() { Id = 1, Description = "a" },
            new() { Id = 2, Description = "b" },
        });

        // Act
        var result = await Endpoints.GetAllItemsAsync(_repository);

        // Assert
        result.Should().BeOfType<Ok<IEnumerable<TodoItemDto>>>()
              .Which.Value.Should().Equal(new TodoItemDto(1, "a"), new TodoItemDto(2, "b"));
    }

    [Fact]
    public async Task GetAllItemsAsync_NoItems_ReturnsEmptyOk()
    {
        // Arrange
        _repository.GetAllItemsAsync().Returns(new List<TodoItem>());

        // Act
        var result = await Endpoints.GetAllItemsAsync(_repository);

        // Assert
        result.Should().BeOfType<Ok<IEnumerable<TodoItemDto>>>()
              .Which.Value.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateItem_EmptyDescription_ReturnsBadRequestWithoutCallingRepository(string? description)
    {
        // Arrange
        var dto = new TodoItemDto(0, description!);

        // Act
        var result = await Endpoints.CreateItem(dto, _repository);

        // Assert
        result.Should().BeOfType<BadRequest<string>>();
        await _repository.DidNotReceiveWithAnyArgs().CreateItem(default!);
    }

    [Fact]
    public async Task CreateItem_ValidDescription_CreatesAndReturnsOk()
    {
        // Arrange
        _repository.CreateItem("new").Returns(new TodoItem { Id = 5, Description = "new" });

        // Act
        var result = await Endpoints.CreateItem(new TodoItemDto(0, "new"), _repository);

        // Assert
        result.Should().BeOfType<Ok<TodoItemDto>>()
              .Which.Value.Should().Be(new TodoItemDto(5, "new"));
        await _repository.Received(1).CreateItem("new");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateItem_EmptyDescription_ReturnsBadRequestWithoutCallingRepository(string? description)
    {
        // Arrange
        var dto = new TodoItemDto(1, description!);

        // Act
        var result = await Endpoints.UpdateItem(1, dto, _repository);

        // Assert
        result.Should().BeOfType<BadRequest<string>>();
        await _repository.DidNotReceiveWithAnyArgs().UpdateItem(default!);
    }

    [Fact]
    public async Task UpdateItem_ValidBody_UpdatesWithMappedModelAndReturnsDto()
    {
        // Arrange
        _repository.UpdateItem(Arg.Any<TodoItem>())
                   .Returns(call => call.Arg<TodoItem>());

        // Act
        var result = await Endpoints.UpdateItem(4, new TodoItemDto(4, "changed"), _repository);

        // Assert
        result.Should().BeOfType<Ok<TodoItemDto>>()
              .Which.Value.Should().Be(new TodoItemDto(4, "changed"));
        await _repository.Received(1).UpdateItem(Arg.Is<TodoItem>(t => t.Id == 4 && t.Description == "changed"));
    }

    [Fact]
    public async Task UpdateItem_ItemNotFound_ReturnsOkWithNullBody()
    {
        // Arrange
        _repository.UpdateItem(Arg.Any<TodoItem>()).Returns((TodoItem)null!);

        // Act
        var result = await Endpoints.UpdateItem(9, new TodoItemDto(9, "x"), _repository);

        // Assert
        result.Should().BeOfType<Ok<TodoItemDto?>>()
              .Which.Value.Should().BeNull();
    }

    [Fact]
    public async Task DeleteItem_DeletesByIdAndReturnsOk()
    {
        // Arrange
        const int id = 8;

        // Act
        var result = await Endpoints.DeleteItem(id, _repository);

        // Assert
        result.Should().BeOfType<Ok>();
        await _repository.Received(1).DeleteItem(id);
    }
}
