using Microsoft.EntityFrameworkCore;
using UserTaskManagementAPI.Abstractions;
using UserTaskManagementAPI.Application.DTOs;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Domain.Errors;
using UserTaskManagementAPI.Persistence.Data;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private const string _dummyHash = "AQAAAAIAAYagAAAAEHf+nRkt+R3P6FRQjAzR2H7HTo2ZCd95CUJ8Ky+AHOxsdLyEqImG0HIvvfED/E8Pqg==";
    public UserService(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserDto?>> RegisterAsync(string username, string role, string password, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == username, cancellationToken))
        {
            return Result.Failure<UserDto?>(UserErrors.UsernameTaken);
        }

        var hashedPassword = _passwordHasher.Hash(password);

        var userResult = User.Create(username, hashedPassword, role);
        if (userResult.Failed)
        {
            return Result.Failure<UserDto?>(userResult.Error!);
        }

        _dbContext.Add(userResult.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToDto(userResult.Value));
    }

    public async Task<Result<UserDto?>> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.Where(u => u.Username == username)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            // Dummy password verification to mitigate timing attacks
            _passwordHasher.Verify(_dummyHash, password);
            return Result.Failure<UserDto?>(UserErrors.InvalidCredentials);
        }
        if (!_passwordHasher.Verify(user.PasswordHash, password))
            return Result.Failure<UserDto?>(UserErrors.InvalidCredentials);

        return Result.Success(MapToDto(user));
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.Select(user => new UserDto(user.Id, user.Username, user.Role)).ToListAsync(cancellationToken);
    }

    private static UserDto? MapToDto(User? user) => user is null ? null : new(user.Id, user.Username, user.Role);

}
