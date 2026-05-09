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
            () => users
        ).WithName("GetUsers");

        app.MapGet(
            "/users/{id}",
            (int id) => users.FirstOrDefault(u => u.Id == id) is User user ? Results.Ok(user) : Results.NotFound()
        ).WithName("GetUserById");

        app.Run();
    }
}