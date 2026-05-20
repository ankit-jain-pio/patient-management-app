using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PatientManagement.API.Middleware;
using Xunit;

namespace PatientManagement.API.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Middleware_ShouldHandleArgumentException_WithBadRequest()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new ArgumentException("Invalid argument");
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Contain("Invalid argument");
    }

    [Fact]
    public async Task Middleware_ShouldHandleKeyNotFoundException_WithNotFound()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new KeyNotFoundException("Resource not found");
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Contain("Resource not found");
    }

    [Fact]
    public async Task Middleware_ShouldHandleUnauthorizedAccessException_WithUnauthorized()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new UnauthorizedAccessException("Access denied");
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Contain("Access denied");
    }

    [Fact]
    public async Task Middleware_ShouldHandleGenericException_WithInternalServerError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new Exception("Unexpected error");
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        responseBody.Should().Contain("Unexpected error");
    }

    [Fact]
    public async Task Middleware_ShouldCallNextDelegate_WhenNoExceptionThrown()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        var nextCalled = false;

        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task Middleware_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate next = (HttpContext ctx) =>
        {
            throw new Exception("Test exception");
        };

        var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
