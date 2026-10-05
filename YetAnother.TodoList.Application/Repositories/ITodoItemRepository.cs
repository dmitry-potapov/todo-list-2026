using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.Application.Repositories;

public interface ITodoItemRepository
{
    Task<List<TodoItem>> GetAllItemsAsync();

    Task<TodoItem> CreateItem(string description);

    Task<TodoItem> UpdateItem(TodoItem item);

    Task DeleteItem(int id);
}