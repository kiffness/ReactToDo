using Todo.Api.Models;

namespace Todo.Api.Interfaces;

public interface ITodoService
{
    public Task<List<TodoModel>> Todos(); 
    public Task<TodoModel?> GetTodo(string id);
    public Task<string?> AddTodo(TodoModel todo);
    public Task<bool> DeleteTodo(string id);
}
