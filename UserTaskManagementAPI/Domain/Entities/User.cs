using UserTaskManagementAPI.Domain.Errors;
using UserTaskManagementAPI.Domain.Rules;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Domain.Entities;

public sealed class User
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = "user";

    //private readonly List<TaskItem> _tasks = [];
    //public IReadOnlyList<TaskItem> Tasks => _tasks.AsReadOnly();

    private User(string username, string passwordHash, string role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }

    public static Result<User> Create(string username, string passwordHash, string role)
    {
        if (username.Length < UserRules.MinUsernameLength || username.Length > UserRules.MaxUsernameLength)
        {
            return Result.Failure<User>(UserErrors.InvalidUsernameLength);
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(UserErrors.EmptyPasswordHash);
        }

        if (!role.Equals("admin", StringComparison.CurrentCultureIgnoreCase) && !role.Equals("user", StringComparison.CurrentCultureIgnoreCase))
        {
            return Result.Failure<User>(UserErrors.InvalidRole);
        }

        return Result.Success(new User(username, passwordHash, role));
    }
}
