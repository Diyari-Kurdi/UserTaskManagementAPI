namespace UserTaskManagementAPI.Application.Requests;

public record RegisterRequest(string Username, string Password, string Role);
