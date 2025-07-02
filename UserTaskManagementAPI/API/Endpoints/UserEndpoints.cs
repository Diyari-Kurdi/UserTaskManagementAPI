using UserTaskManagementAPI.Abstractions;
using UserTaskManagementAPI.Application.Requests;
using UserTaskManagementAPI.Domain.Errors;
using UserTaskManagementAPI.Services;

namespace UserTaskManagementAPI.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth/");

        group.MapPost("register", async (RegisterRequest request, IUserService userService) =>
        {
            var resut = await userService.RegisterAsync(request.Username, request.Role, request.Password);
            if (resut.Failed)
            {
                return Results.BadRequest(resut.Error!);
            }
            return Results.Ok("User registered");
        });

        group.MapPost("login", async (LoginRequest request, IUserService userService, TokenService tokenService, IConfiguration config) =>
        {
            var loginResult = await userService.LoginAsync(request.Username, request.Password);

            if (loginResult.Failed)
            {
                if (loginResult.Error! == UserErrors.InvalidCredentials)
                {
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.BadRequest(loginResult.Error!);
                }
            }

            return Results.Ok(new
            {
                token = tokenService.GenerateToken(loginResult.Value!)
            });
        });
    }
}
