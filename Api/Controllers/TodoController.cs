using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class TodoController : BaseApiController
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoController> _logger;

        public TodoController(TodoContext context, ILogger<TodoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Todo>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<Todo>>> GetTodos()
        {
            try
            {
                var todos = await _context.Todos.ToListAsync();

                return todos;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(typeof(Todo), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Todo>> GetTodo(string id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);

                if (todo == null) return NotFound();

                return Ok(todo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("CreateTodo")]
        [ProducesResponseType(typeof(Todo), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateTodo([FromQuery] string todoTitle)
        {
            try
            {
                var todo = new Todo
                {
                    Title = todoTitle
                };
                await _context.Todos.AddAsync(todo);
                var result = await _context.SaveChangesAsync() > 0;

                if (result) return CreatedAtRoute("GetById", new { id = todo.Id }, todo);

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
                var todo = await _context.Todos.FindAsync(id);

                if (todo == null) return NotFound();

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
    }
}