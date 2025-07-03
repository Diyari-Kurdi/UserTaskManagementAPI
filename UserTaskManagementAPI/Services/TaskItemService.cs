using Microsoft.EntityFrameworkCore;
using System.Data;
using UserTaskManagementAPI.Abstractions;
using UserTaskManagementAPI.Application.DTOs;
using UserTaskManagementAPI.Application.Requests;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Persistence.Data;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Services;

public class TaskItemService : ITaskItemService
{
    private readonly AppDbContext _dbContext;

    public TaskItemService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<TaskItem>> CreateAsync(CreateTaskRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
    {
        var taskItemResult = TaskItem.Create(request.Title, request.Description, request.Tags, request.Priority, createdByUserId);
        if (taskItemResult.Failed)
        {
            return Result.Failure<TaskItem>(taskItemResult.Error!);
        }
        _dbContext.TaskItems.Add(taskItemResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(taskItemResult.Value);
    }

    public async Task<List<TaskItem>> GetAllAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TaskItems.AsQueryable();

        if (!role.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
        {
            query = query.Where(t => t.CreatedByUserId == userId);
        }


        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    { 
        return await _dbContext.TaskItems.Where(t => t.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetFilteredWithPagingAsync(Guid userId,
                                                                 string role,
                                                                 int pageNumber,
                                                                 int pageSize,
                                                                 TaskItemFilterDto taskItemFilter,
                                                                 CancellationToken cancellationToken = default)
    {
        return await BuildFilteredQuery(userId, role, taskItemFilter)
                          .Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetFilteredAsync(Guid userId,
                                                       string role,
                                                       TaskItemFilterDto taskItemFilter,
                                                       CancellationToken cancellationToken = default)
    {
        return await BuildFilteredQuery(userId, role, taskItemFilter).AsNoTracking().ToListAsync(cancellationToken);
    }

    private IQueryable<TaskItem> BuildFilteredQuery(Guid userId, string role, TaskItemFilterDto filter)
    {
        var query = _dbContext.TaskItems.AsQueryable();

        if (!role.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
        {
            query = query.Where(t => t.CreatedByUserId == userId);
        }

        if (filter.IsCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == filter.IsCompleted.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            query = query.Where(t => t.Tags.Contains(filter.Tag));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(t => t.Title.Contains(filter.Search)
                                  || (t.Description != null && t.Description.Contains(filter.Search)));
        }
        if (filter.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == filter.Priority);
        }

        if (filter.SortByPriority.HasValue)
        {
            query = filter.SortByPriority == Domain.Enums.SortType.Descending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority);
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        return query;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.Tags = request.Tags;
        task.Priority = request.Priority;

        _dbContext.TaskItems.Update(task);

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return task;
    }
    public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _dbContext.TaskItems.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
