using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NuGet.Frameworks;
using Todo.Api.Controllers;
using Todo.Api.Data;
using Todo.Api.Interfaces;
using Todo.Api.Models;
using Todo.Api.Services;

namespace Todo.Tests;

public class TodoControllerTests
{
    private ITodoService _todoService;
    private TodoController _todoController;

    [SetUp]
    public void Setup()
    {
        // Arrange: Create a mocked ITodoService
        _todoService = Substitute.For<ITodoService>();

        // Arrange: Create an instance of TodoController with the mocked service
        _todoController = new TodoController(_todoService);
    }

    [Test]
    public async Task Api_Todo_Should_Return_200_Ok()
    {
        // Arrange: Set up the mock service to return a list of TodoModel
        var expectedTodos = new List<TodoModel>
        {
            new TodoModel { Title = "Todo 1" },
            new TodoModel { Title = "Todo 2" }
        };

        _todoService.Todos().Returns(await Task.FromResult(expectedTodos));

        // Act: Call the GetTodos Method
        var result = await _todoController.GetTodos();

        // Assert: Check that the result is an ActionResult<List<TodoModel>>
        Assert.IsInstanceOf<ActionResult<List<TodoModel>>>(result);

        // Assert: That result is not null
        Assert.That(result.Result, !Is.Null);

        var okResult = (OkObjectResult)result.Result;

        // Assert: That result value is not null
        Assert.That(okResult.Value, !Is.Null);

        var actualTodos = (List<TodoModel>)okResult.Value;
        CollectionAssert.AreEqual(expectedTodos, actualTodos);
    }

    [Test]
    public async Task Api_Todo_Should_Return_500_ServerError()
    {
        // Arrange: Set up the mock service to throw an exception
        _todoService.Todos().Throws(new Exception("Something went wrong"));

        //Act: Call the GetTodos method
        var result = await _todoController.GetTodos();

        // Assert: Check that the result is an ActionResult<List<TodoModel>>
        Assert.That(result, Is.InstanceOf<ActionResult<List<TodoModel>>>());

        // Assert: That result is not null
        Assert.That(result.Result, !Is.Null);

        var objectResult = (ObjectResult)result.Result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task Api_Get_Todo_Should_Return_200_Ok()
    {
        // Arrange: Set up the mock service to return a TodoModel
        var guid = Guid.NewGuid().ToString();
        var expectedTodo = new TodoModel { Id = guid, Title = "Todo 1" };

        _todoService.GetTodo(guid).Returns(await Task.FromResult(expectedTodo));

        // Act: Call the GetTodo Method
        var result = await _todoController.GetTodo(guid);

        //Assert: Check that the result is an ActionResult<TodoModel>
        Assert.That(result, Is.InstanceOf<ActionResult<TodoModel>>());

        // Assert: That result is not null
        Assert.That(result.Result, !Is.Null);
        var okResult = (OkObjectResult)result.Result;

        // Assert: That the result value is not null
        Assert.That(okResult.Value, !Is.Null);

        // Assert: that the Id mataches
        var actualTodo = (TodoModel)okResult.Value;
        Assert.That(actualTodo.Id, Is.EqualTo(guid));

        // Assert: that the status code is 200
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Api_GetTodo_Should_Return_404_NotFound()
    {
        // Arrange: Set up the mock service to return null
        var guid = Guid.NewGuid().ToString();
        _todoService.GetTodo(guid).Returns(await Task.FromResult<TodoModel?>(null));

        // Act: Call the GetTodo Method
        var result = await _todoController.GetTodo(guid);

        //Assert: Check that the result is an ActionResult<TodoModel>
        Assert.That(result, Is.InstanceOf<ActionResult<TodoModel>>());

        // Assert: That result is null
        Assert.That(result.Value, Is.Null);

        // Assert: The object is not null
        Assert.That(result.Result, !Is.Null);

        // Assert: That the status code is 404
        var objectResult = (NotFoundResult)result.Result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task Api_GetTodo_Should_Return_500_InternalServerError()
    {
        // Arrange: Set up the mock service to throw an exception
        var guid = Guid.NewGuid().ToString();
        _todoService.GetTodo(guid).Throws(new Exception("Something went wrong"));

        //Act: Call the GetTodos method
        var result = await _todoController.GetTodo(guid);

        // Assert: Check that the result is an ActionResult<List<TodoModel>>
        Assert.That(result, Is.InstanceOf<ActionResult<TodoModel>>());

        // Assert: That result is not null
        Assert.That(result.Result, !Is.Null);

        var objectResult = (ObjectResult)result.Result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task Api_CreateTodo_Should_Return_201_CreatedAtRoute()
    {
            // Arrange: Create the expected TodoModel (with Title only)
            var expectedTitle = "Todo 1";

            // Mock the AddTodo method to return the Id from the created TodoModel
            _todoService.AddTodo(Arg.Any<TodoModel>())
                        .Returns(info => Task.FromResult((info.Arg<TodoModel>()).Id));

            // Act: Call the CreateTodo method
            var result = await _todoController.CreateTodo(expectedTitle) as CreatedAtRouteResult;

            // Assert: Check that the result is not null and has the correct status code
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            // Extract the created TodoModel for verification
            var createdTodo = result.Value as TodoModel;
            Assert.That(createdTodo, !Is.Null);
            Assert.That(createdTodo.Id, !Is.Null);

            // Assert: Verify the route values
            Assert.That(result.RouteName, Is.EqualTo("GetById"));
            Assert.That(result.RouteValues, Is.Not.Null);
            Assert.That(result.RouteValues["id"], Is.EqualTo(createdTodo.Id));
            Assert.That(createdTodo.Title, Is.EqualTo(expectedTitle));
    }

    [Test]
    public async Task Api_CreateTodo_Returns_Bad_Request()
    {
        _todoService.AddTodo(Arg.Any<TodoModel>()).Returns(await Task.FromResult(string.Empty));

        // Act
        var result = await _todoController.CreateTodo(todoTitle: "Todo 1");

        // Assert: Check that the result is null
        Assert.That(result, !Is.Null);

        // Assert: That result is type of BadRequestObjectResult
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var resultObject = result as BadRequestObjectResult;
        Assert.That(resultObject, !Is.Null);
        Assert.That(resultObject.Value, Is.InstanceOf<ProblemDetails>());
        Assert.That(resultObject.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Api_CreateTodo_Returns_500_ServerError()
    {
        // Arrange: Set up the mock service to throw an exception
        _todoService.AddTodo(Arg.Any<TodoModel>()).ThrowsAsync(new Exception("Something went wrong"));

        //Act: Call the GetTodos method
        var result = await _todoController.CreateTodo(todoTitle: "Todo 1");

        Assert.That(result, Is.InstanceOf<ObjectResult>());

        var objectResult = (ObjectResult)result;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task Api_Delete_Todo_Should_Return_200_Ok()
    {
        // Arrange: Set up the mock service to return a TodoModel
        var guid = Guid.NewGuid().ToString();

        _todoService.DeleteTodo(guid).ReturnsForAnyArgs(true);

        // Act: Call the GetTodo Method
        var result = await _todoController.DeleteTodo(id: guid);

        //Assert: Check that the result is an ActionResult<TodoModel>
        Assert.That(result, Is.InstanceOf<OkResult>());

        // Assert: That result is not null
        var okResult = (OkResult)result;

        // // Assert: that the status code is 200
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Api_Delete_Todo_Should_Return_404_NotFound()
    {
        // Arrange: Set up the mock service to return a TodoModel
        var guid = Guid.NewGuid().ToString();

        _todoService.DeleteTodo(guid).ReturnsForAnyArgs(false);

        // Act: Call the GetTodo Method
        var result = await _todoController.DeleteTodo(id: guid);

        //Assert: Check that the result is an ActionResult<TodoModel>
        Assert.That(result, Is.InstanceOf<NotFoundResult>());

        // Assert: That result is not null
        var okResult = (NotFoundResult)result;

        // // Assert: that the status code is 200
        Assert.That(okResult.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task Api_Delete_Todo_Should_Return_500_ServerError()
    {
        // Arrange: Set up the mock service to return a TodoModel
        var guid = Guid.NewGuid().ToString();

        _todoService.DeleteTodo(guid).ThrowsForAnyArgs(new Exception("Something went wrong"));

        // Act: Call the GetTodo Method
        var result = await _todoController.DeleteTodo(id: guid);

        //Assert: Check that the result is an ActionResult<TodoModel>
        Assert.That(result, Is.InstanceOf<ObjectResult>());

        // Assert: That result is not null
        var okResult = (ObjectResult)result;

        // // Assert: that the status code is 200
        Assert.That(okResult.StatusCode, Is.EqualTo(500));
    }
}
