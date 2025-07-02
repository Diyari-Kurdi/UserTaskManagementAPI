using UserTaskManagementAPI.Application.DTOs;
using UserTaskManagementAPI.Shared;

namespace UserTaskManagementAPI.Abstractions;
public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDto?>> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<Result<UserDto?>> RegisterAsync(string username, string role, string password, CancellationToken cancellationToken = default);
}