using Microsoft.EntityFrameworkCore;
using YetAnother.TodoList.Application.Repositories;
using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.Infrastructure;

public class TodoItemRepository(TodoDbContext dbContext) : ITodoItemRepository
{
    public async Task<List<TodoItem>> GetAllItemsAsync()
    {
        return await dbContext.Todos.ToListAsync();
    }

    public async Task<TodoItem> CreateItem(string description)
    {
        var newItem = new TodoItem()
        {
            Description = description
        };
        
        await dbContext.Todos.AddAsync(newItem);
        await dbContext.SaveChangesAsync();

        return newItem;
    }

    public async Task<TodoItem> UpdateItem(TodoItem item)
    {
        var existing = dbContext.Todos.FirstOrDefault(t => t.Id == item.Id);
        if (existing != null)
        {
            existing.Description = item.Description;
            dbContext.Todos.Update(existing);
            await dbContext.SaveChangesAsync();   
        }

        return existing!;
    }

    public async Task DeleteItem(int id)
    {
        var item = await dbContext.Todos.SingleOrDefaultAsync(t => t.Id == id);

        if (item != null)
        {
            dbContext.Todos.Remove(item);
            await dbContext.SaveChangesAsync();
        }
    }
}

