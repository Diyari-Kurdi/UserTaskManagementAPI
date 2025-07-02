using UserTaskManagementAPI.Application.DTOs;
using UserTaskManagementAPI.Application.Requests;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Abstractions;
public interface ITaskItemService
{
    Task<Result<TaskItem>> CreateAsync(CreateTaskRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetFilteredAsync(Guid userId, string role, TaskItemFilterDto taskItemFilter, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateAsync(TaskItem task, UpdateTaskRequest request, CancellationToken cancellationToken = default);
}