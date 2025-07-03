using UserTaskManagementAPI.Domain.Enums;
using UserTaskManagementAPI.Domain.Errors;
using UserTaskManagementAPI.Domain.Rules;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public Guid CreatedByUserId { get; init; }
    public string[] Tags { get; set; } = [];

    public TaskPriority Priority { get; set; } = TaskPriority.Low;

    private TaskItem(string title,
                     string? description,
                     string[] tags,
                     TaskPriority priority,
                     Guid createdByUserId)
    {
        Id = Guid.CreateVersion7();
        Title = title;
        Description = description;
        Tags = tags;
        Priority = priority;
        CreatedByUserId = createdByUserId;
    }

    public static Result<TaskItem> Create(string title,
                                          string? description,
                                          string[] tags,
                                          TaskPriority priority,
                                          Guid createdByUserId)
    {
        if (!IsUserIdValid(createdByUserId))
        {
            return Result.Failure<TaskItem>(TaskItemErrors.InvalidUserId);
        }

        if (!IsTitleValid(title))
        {
            return Result.Failure<TaskItem>(TaskItemErrors.InvalidTitleLength);
        }

        if (!IsDescriptionValid(description))
        {
            return Result.Failure<TaskItem>(TaskItemErrors.DescriptionTooLong);
        }

        return Result.Success(new TaskItem(title, description, tags, priority, createdByUserId));
    }

    private static bool IsUserIdValid(Guid id) => id != Guid.Empty;

    private static bool IsTitleValid(string title) =>
        title.Length >= TaskItemRules.MinTitleLength &&
        title.Length <= TaskItemRules.MaxTitleLength;

    private static bool IsDescriptionValid(string? desc) =>
        desc == null || desc.Length <= TaskItemRules.MaxDescriptionLength;
}