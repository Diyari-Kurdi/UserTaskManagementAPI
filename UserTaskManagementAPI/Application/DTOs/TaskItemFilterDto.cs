using UserTaskManagementAPI.Domain.Enums;

namespace UserTaskManagementAPI.Application.DTOs;

public sealed record TaskItemFilterDto(int PageNumber,
                                       int PageSize,
                                       bool? IsCompleted,
                                       string? Tag,
                                       string? Search,
                                       TaskPriority? Priority,
                                       SortType? SortByPriority = SortType.Descending);

