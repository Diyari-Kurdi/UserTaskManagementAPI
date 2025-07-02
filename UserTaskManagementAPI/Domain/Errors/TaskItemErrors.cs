using UserTaskManagementAPI.Domain.Rules;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Domain.Errors;

public static class TaskItemErrors
{
    public static readonly Error InvalidUserId = new("User.InvalidUserId", "Invalid User");
    public static readonly Error InvalidTitleLength = new("User.InvalidTitleLength", $"Title must be between {TaskItemRules.MinTitleLength} and {TaskItemRules.MaxTitleLength} characters long!");
    public static readonly Error DescriptionTooLong = new("User.LongDescription", $"Description must be less than {TaskItemRules.MaxDescriptionLength} characters!");
}
