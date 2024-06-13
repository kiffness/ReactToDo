using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Api.Models;

namespace Todo.Api.Data;

public class DbInitializer
{
    public static void Initialize(TodoContext context)
    {
        if (context.Todos.Any()) return;

        var todos = new List<TodoModel>
        {
            new TodoModel
            {
                Title = "Learn React",
            },
            new TodoModel
            {
                Title = "Wash Dishes",
            },
            new TodoModel
            {
                Title = "Do the washing",
            },
            new TodoModel
            {
                Title = "Learn Typescript",
            },
            new TodoModel
            {
                Title = "Go to work",
            },
            new TodoModel
            {
                Title = "Brush teeth",
            },
        };

        context.AddRange(todos);
        context.SaveChanges();
    }
}
