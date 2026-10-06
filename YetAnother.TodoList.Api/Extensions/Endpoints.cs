using YetAnother.TodoList.Application.Dtos;
using YetAnother.TodoList.Application.Extensions.Mapping;
using YetAnother.TodoList.Application.Repositories;

namespace YetAnother.TodoList.Api.Extensions;

// Could also create a service as an extra layer between API handlers and repository
public static class Endpoints
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/todos")
                       .WithTags("Todos");

        group.MapGet("/", GetAllItemsAsync)
             .Produces<List<TodoItemDto>>(StatusCodes.Status200OK);

        group.MapPost("/", CreateItem)
             .Produces<TodoItemDto>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:int}", UpdateItem)
             .Produces<TodoItemDto>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:int}", DeleteItem)
             .Produces(StatusCodes.Status200OK);

        return app;
    }

    internal static async Task<IResult> GetAllItemsAsync(ITodoItemRepository todoItemRepository)
    {
        return TypedResults.Ok(
            (await todoItemRepository.GetAllItemsAsync()).Select(g => g.ToDto())
        );
    }

    internal static async Task<IResult> CreateItem(TodoItemDto dto, ITodoItemRepository todoItemRepository)
    {
        if (string.IsNullOrEmpty(dto.Description))
        {
            return TypedResults.BadRequest("The description should not be empty!");
        }

        var result = (await todoItemRepository.CreateItem(dto.Description)).ToDto();

        return TypedResults.Ok(result);
    }

    internal static async Task<IResult> UpdateItem(int id, TodoItemDto dto, ITodoItemRepository todoItemRepository)
    {
        if (string.IsNullOrEmpty(dto?.Description))
        {
            return TypedResults.BadRequest("The description should not be empty!");
        }

        var result = (await todoItemRepository.UpdateItem(dto.ToModel()))?.ToDto();

        return TypedResults.Ok(result);
    }

    internal static async Task<IResult> DeleteItem(int id, ITodoItemRepository todoItemRepository)
    {
        await todoItemRepository.DeleteItem(id);

        return TypedResults.Ok();
    }
}
