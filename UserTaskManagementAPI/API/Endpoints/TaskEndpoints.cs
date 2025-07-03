using System.Security.Claims;
using UserTaskManagementAPI.Abstractions;
using UserTaskManagementAPI.Application.DTOs;
using UserTaskManagementAPI.Application.Requests;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Domain.Enums;

namespace UserTaskManagementAPI.API.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tasks/").RequireAuthorization();

        group.MapGet("", async (ClaimsPrincipal user,
                        ITaskItemService taskService,
                        int? pageNumber,
                        int? pageSize,
                        bool? isCompleted,
                        string? tag,
                        string? search,
                        TaskPriority? priority,
                        SortType? sortByPriority,
                        CancellationToken cancellationToken) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = user.FindFirstValue(ClaimTypes.Role)!;

            List<TaskItem> items = [];
            var filter = new TaskItemFilterDto(isCompleted, tag, search, priority, sortByPriority);

            if (pageNumber.HasValue ^ pageSize.HasValue)
            {
                return Results.BadRequest("Both pageNumber and pageSize must be provided together for pagination.");
            }

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                items = await taskService.GetFilteredWithPagingAsync(userId, role, pageNumber.Value, pageSize.Value, filter, cancellationToken);
            }
            else if (isCompleted.HasValue
                    || !string.IsNullOrWhiteSpace(tag)
                    || !string.IsNullOrWhiteSpace(search)
                    || priority.HasValue
                    || sortByPriority.HasValue)
            {
                items = await taskService.GetFilteredAsync(userId, role, filter, cancellationToken);
            }
            else
            {
                items = await taskService.GetAllAsync(userId, role, cancellationToken);
            }
            return Results.Ok(items);
        });

        group.MapPost("", async (ClaimsPrincipal user, CreateTaskRequest taskRequest, ITaskItemService taskService, CancellationToken cancellationToken = default) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await taskService.CreateAsync(taskRequest, userId, cancellationToken);
            if (result.Failed)
            {
                return Results.BadRequest(result.Error!);
            }
            return Results.Ok(result.Value);
        });

        group.MapPut("{id:Guid}", async (Guid id,
                                         ClaimsPrincipal user,
                                         UpdateTaskRequest request,
                                         ITaskItemService taskService,
                                         CancellationToken cancellationToken = default) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = user.FindFirstValue(ClaimTypes.Role)!;

            var task = await taskService.GetByIdAsync(id, cancellationToken);


            if (task == null)
                return Results.NotFound();

            if (!role.Equals("admin", StringComparison.CurrentCultureIgnoreCase) && task.CreatedByUserId != userId)
                return Results.Forbid();


            var updated = await taskService.UpdateAsync(task, request, cancellationToken);
            return Results.Ok(updated);
        });

        group.MapDelete("{id:Guid}", async (Guid id, ClaimsPrincipal user, ITaskItemService taskService, CancellationToken cancellationToken = default) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = user.FindFirstValue(ClaimTypes.Role)!;

            var task = await taskService.GetByIdAsync(id, cancellationToken);

            if (task == null)
                return Results.NotFound();

            if (!role.Equals("admin", StringComparison.CurrentCultureIgnoreCase) && task.CreatedByUserId != userId)
                return Results.Forbid();

            await taskService.DeleteAsync(task, cancellationToken);
            return Results.NoContent();
        });

    }
}
