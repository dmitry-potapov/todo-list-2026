using FluentAssertions;
using YetAnother.TodoList.Application.Dtos;
using YetAnother.TodoList.Application.Extensions.Mapping;
using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.UnitTests.Application.Extensions.Mapping;

public class TodoItemMappingExtensionsTests
{
    [Fact]
    public void ToDto_MapsIdAndDescription()
    {
        // Arrange
        var model = new TodoItem { Id = 7, Description = "Buy milk" };

        // Act
        var dto = model.ToDto();

        // Assert
        dto.Should().Be(new TodoItemDto(7, "Buy milk"));
    }

    [Fact]
    public void ToModel_MapsIdAndDescription()
    {
        // Arrange
        var dto = new TodoItemDto(3, "Walk the dog");

        // Act
        var model = dto.ToModel();

        // Assert
        model.Id.Should().Be(3);
        model.Description.Should().Be("Walk the dog");
    }

    [Fact]
    public void RoundTrip_PreservesValues()
    {
        // Arrange
        var dto = new TodoItemDto(11, "Round trip");

        // Act
        var result = dto.ToModel().ToDto();

        // Assert
        result.Should().Be(dto);
    }
}
