using UserTaskManagementAPI.Domain.Enums;

namespace UserTaskManagementAPI.Application.Requests;

public sealed record UpdateTaskRequest(string Title,
                                       string? Description,
                                       string[] Tags,
                                       TaskPriority Priority,
                                       bool IsCompleted);