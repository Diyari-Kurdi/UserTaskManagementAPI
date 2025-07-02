namespace UserTaskManagementAPI.Domain.Rules;

public static class TaskItemRules
{
    public const int MinTitleLength = 2;
    public const int MaxTitleLength = 250;

    public const int MaxDescriptionLength = 500;
}
