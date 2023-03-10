using Microsoft.EntityFrameworkCore;

namespace NetTodoApi.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}