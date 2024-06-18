using Microsoft.EntityFrameworkCore;
using Todo.Api.Models;

namespace Todo.Api.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions options) : base(options)
    {

    }

    public virtual DbSet<TodoModel> Todos { get; set; }
}
