namespace UserTaskManagementAPI.Shared;

public record Error(string Code,string Message)
{
    public static readonly Error NullValue = new(string.Empty, "NullValue");
}