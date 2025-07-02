using UserTaskManagementAPI.Domain.Rules;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Domain.Errors;

public static class UserErrors
{
    public static readonly Error InvalidUsernameFormat = new("User.InvalidUsernameFormat", "Username can only contain letters, numbers, and underscores, and must not start with a number.");

    public static readonly Error InvalidUsernameLength = new("User.InvalidUsernameLength", $"Username must be between {UserRules.MinUsernameLength} and {UserRules.MaxUsernameLength} characters long!");

    public static readonly Error EmptyPasswordHash = new("User.EmptyPasswordHash", "Password hash must not be empty!");

    public static readonly Error InvalidRole = new("User.InvalidRole", "Invalid User Role");

    public static readonly Error UsernameTaken = new("User.InvalidUsername", "Username already taken");
    public static readonly Error InvalidCredentials = new("User.Credentials", "Invalid username or password");

}
