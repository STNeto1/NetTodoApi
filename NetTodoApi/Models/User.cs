namespace NetTodoApi.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public List<TodoItem> TodoItems { get; set; }
}