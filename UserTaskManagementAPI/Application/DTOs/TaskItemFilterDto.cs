using UserTaskManagementAPI.Domain.Enums;

namespace UserTaskManagementAPI.Application.DTOs;

public sealed record TaskItemFilterDto(bool? IsCompleted,
                                       string? Tag,
                                       string? Search,
                                       TaskPriority? Priority,
                                       SortType? SortByPriority = SortType.Descending);