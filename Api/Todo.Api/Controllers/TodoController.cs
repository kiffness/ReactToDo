using Microsoft.AspNetCore.Mvc;
using Todo.Api.Data;
using Todo.Api.Interfaces;
using Todo.Api.Models;

namespace Todo.Api.Controllers;

public class TodoController : BaseApiController
{
    private readonly ITodoService _todo;

    public TodoController(ITodoService todo)
    {
        _todo = todo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TodoModel>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<TodoModel>>> GetTodos()
    {
        try
        {
            var todos = await _todo.Todos();

            return Ok(todos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpGet("{id}", Name = "GetById")]
    [ProducesResponseType(typeof(TodoModel), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TodoModel>> GetTodo(string id)
    {
        try
        {
            var todo = await _todo.GetTodo(id);

            if (todo == null) return NotFound();

            return Ok(todo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost("CreateTodo")]
    [ProducesResponseType(typeof(TodoModel), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CreateTodo([FromQuery] string todoTitle)
    {
        try
        {
            var todo = new TodoModel
            {
                Title = todoTitle
            };

            var result = await _todo.AddTodo(todo);

            if (!string.IsNullOrEmpty(result)) return CreatedAtRoute("GetById", new { id = todo.Id }, todo);

            return BadRequest(new ProblemDetails { Title = "Problem saving tood to database" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteTodo(string id)
    {
        try
        {
            var result = await _todo.DeleteTodo(id);

            if (!result) return NotFound();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }

    }
}
