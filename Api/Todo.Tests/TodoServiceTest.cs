using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Todo.Api.Data;
using Todo.Api.Interfaces;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Tests;

public class TodoServiceTest
{
    private ITodoService _todoService;
    private TodoContext _mockedDbContext;

    [SetUp]
    public void SetUp()
    {
        // Arrange: Create a mocked TodoContext
        var dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mockedDbContext = Create.MockedDbContextFor<TodoContext>(dbContextOptions);

        // Arrange: Create an instance of TodoService with the mocked context
        _todoService = Substitute.For<TodoService>(_mockedDbContext);
    }

    [Test]
    public async Task Todos_Should_Return_Items()
    {
        // Arrange
        var newTodo = new TodoModel { Title = "Test 1" };
        await _mockedDbContext.Todos.AddAsync(newTodo);
        await _mockedDbContext.SaveChangesAsync();

        // Act
        var actualTodos = await _todoService.Todos();

        // Assert
        Assert.That(actualTodos.Count(), Is.EqualTo(await _mockedDbContext.Todos.CountAsync()));
        Assert.That(actualTodos.Count(), !Is.EqualTo(0));
        Assert.That(actualTodos, !Is.Null);
    }

    [Test]
    public async Task Todos_Should_Return_0()
    {
        // Act
        var actualTodos = await _todoService.Todos();

        // Assert
        Assert.That(actualTodos.Count(), Is.EqualTo(await _mockedDbContext.Todos.CountAsync()));
        Assert.That(actualTodos.Count(), Is.EqualTo(0));
        Assert.That(actualTodos, !Is.Null);
    }

    [Test]
    public async Task GetTodo_Should_Return_Requested_Todo()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
        var todos = new List<TodoModel>
        {
            new TodoModel { Id = guid, Title = "Test 1" },
            new TodoModel { Title = "Test 2" }
        };

        _mockedDbContext.Todos.AddRange(todos);

        // Act
        var actualTodo = await _todoService.GetTodo(guid);

        // Assert
        Assert.That(actualTodo, !Is.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actualTodo.Title, Is.EqualTo("Test 1"));
            Assert.That(actualTodo.Id, Is.EqualTo(guid));
        });
    }

    [Test]
    public async Task GetTodo_Should_Return_Null_When_Todo_Not_Found()
    {
        var actualTodo = await _todoService.GetTodo("Nothing");

        Assert.That(actualTodo, Is.Null);
    }

    [Test]
    public async Task AddTodo_Should_Return_Null_When_SaveChangesAsync_Fails()
    {
        var todo = new TodoModel { Title = "Test Todo" };

        _mockedDbContext.SaveChangesAsync().Returns(Task.FromResult(0));

        var result = await _todoService.AddTodo(todo);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task AddTodo_Should_Return_TodoId_When_SaveChangesAsync_Succeeds()
    {
        var guid = Guid.NewGuid().ToString();
        var todo = new TodoModel { Id = guid, Title = "Test" };

        _mockedDbContext.SaveChangesAsync().Returns(1);

        var result = await _todoService.AddTodo(todo);

        Assert.That(result, !Is.Null);
        Assert.That(result, Is.EqualTo(guid));
    }

    [Test]
    public async Task DeleteTodo_Should_Return_False_When_SaveChangesAsync_Fails()
    {
        _mockedDbContext.SaveChangesAsync().Returns(Task.FromResult(0));

        var result = await _todoService.DeleteTodo(Guid.NewGuid().ToString());

        Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public async Task DeleteTodo_Should_Return_True_When_SaveChangesAsync_Succeeds()
    {
        var guid = Guid.NewGuid().ToString();
        var todo = new TodoModel { Id = guid, Title = "Test" };

        await _mockedDbContext.Todos.AddAsync(todo);
        await _mockedDbContext.SaveChangesAsync();

        _mockedDbContext.SaveChangesAsync().Returns(Task.FromResult(1));

        var result = await _todoService.DeleteTodo(guid);

        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public async Task DeleteTodo_Should_Return_False_When_FindAsync_Is_Null()
    {
        _mockedDbContext.Todos.FindAsync(Arg.Any<string>()).Returns(await Task.FromResult<TodoModel?>(null));

        var result = await _todoService.DeleteTodo(Guid.NewGuid().ToString());

        Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public async Task DeleteTodo_Should_Return_False_When_Id_Is_Not_In_Table()
    {
        var result = await _todoService.DeleteTodo(Guid.NewGuid().ToString());

        Assert.That(result, Is.EqualTo(false));
    }


    [TearDown]
    public void TearDown()
    {
        _mockedDbContext.Dispose();
    }
}
