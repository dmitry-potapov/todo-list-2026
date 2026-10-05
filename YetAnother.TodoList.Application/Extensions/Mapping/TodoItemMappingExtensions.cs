using YetAnother.TodoList.Application.Dtos;
using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.Application.Extensions.Mapping;

public static class TodoItemMappingExtensions
{
    extension (TodoItem model)
    {
        public TodoItemDto ToDto() => new(model.Id, model.Description);
    }

    extension (TodoItemDto dto)
    {
        public TodoItem ToModel() => new() { Id = dto.Id, Description = dto.Description };
    }
}
