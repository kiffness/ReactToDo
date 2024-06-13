using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Todo> Todos { get; set; }
}
