using UserTaskManagementAPI.Domain.Enums;

namespace UserTaskManagementAPI.Application.Requests;

public sealed record CreateTaskRequest(string Title,
                                       string? Description,
                                       string[] Tags,
                                       TaskPriority Priority);