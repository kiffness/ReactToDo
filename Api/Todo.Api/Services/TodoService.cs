using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Data;
using Todo.Api.Interfaces;
using Todo.Api.Models;

namespace Todo.Api.Services;

public class TodoService(TodoContext context) : ITodoService
{
    private readonly TodoContext _context = context;

    public async Task<List<TodoModel>> Todos()
        => await _context.Todos.ToListAsync();

    public async Task<TodoModel?> GetTodo(string id) 
     => await _context.Todos.FindAsync(id);

    public async Task<string?> AddTodo(TodoModel todo)
    {
        await _context.Todos.AddAsync(todo);
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return todo.Id;
        return null;
    }

    public async Task<bool> DeleteTodo(string id)
    {
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null) return false;

        _context.Todos.Remove(todo);
        var result  = await _context.SaveChangesAsync() > 0;

        return result;
    }
}
