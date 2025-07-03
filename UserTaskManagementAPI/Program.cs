using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using UserTaskManagementAPI.Abstractions;
using UserTaskManagementAPI.API.Endpoints;
using UserTaskManagementAPI.API.Extensions;
using UserTaskManagementAPI.Domain.Enums;
using UserTaskManagementAPI.Persistence.Data;
using UserTaskManagementAPI.Services;

namespace UserTaskManagementAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;


        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


        builder.Services.AddJwtAuthentication(configuration);
        builder.Services.AddAuthorization();
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "UserTask API", Version = "v1" });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            o.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            o.MapType<SortType>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = [.. Enum.GetNames<SortType>().Select(name => new OpenApiString(name))]
            });
            o.MapType<TaskPriority>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = [.. Enum.GetNames<TaskPriority>().Select(name => new OpenApiString(name))]
            });
        });

        builder.Services.AddScoped<TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITaskItemService, TaskItemService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapTaskEndpoints();
        app.MapUserEndpoints();

        app.Run();
    }
}
