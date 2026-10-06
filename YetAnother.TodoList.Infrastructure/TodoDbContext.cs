using Microsoft.EntityFrameworkCore;
using YetAnother.TodoList.Domain;

namespace YetAnother.TodoList.Infrastructure;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
