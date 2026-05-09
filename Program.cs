using System.Collections.Generic;

namespace backend;

class User
{
    public int Id { get; set; }
    public string Name { get; set; }

    public User(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class Program
{
    public record UserRequest(string Name);

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/helloWorld", () => "Hello World!").WithName("GetHelloWorld");

        var users = new List<User>
        {
            new User(1, "Alice"),
            new User(2, "Bob"),
            new User(3, "Charlie")
        };

        app.MapGet(
            "/users",
            // Users UserRequest to return only Name property of User
            () => users.Select(u => new UserRequest(u.Name))
        ).WithName("GetUsers");

        app.MapGet(
            "/users/{id}",
            (int id) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);

                if (user is User foundUser)
                {
                    return Results.Ok(new UserRequest(foundUser.Name));
                }

                return Results.NotFound();
            }
        ).WithName("GetUserById");

        app.Run();
    }
}