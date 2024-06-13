using Api.Models;

namespace Api.Data;

public class DbInitializer
{
    public static void Initialize(TodoContext context)
    {
        if (context.Todos.Any()) return;

        var todos = new List<Todo>
        {
            new Todo
            {
                Title = "Learn React",
            },
            new Todo
            {
                Title = "Wash Dishes",
            },
            new Todo
            {
                Title = "Do the washing",
            },
            new Todo
            {
                Title = "Learn Typescript",
            },
            new Todo
            {
                Title = "Go to work",
            },
            new Todo
            {
                Title = "Brush teeth",
            },
        };

        context.AddRange(todos);
        context.SaveChanges();
    }
}
