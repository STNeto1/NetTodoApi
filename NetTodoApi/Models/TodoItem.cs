namespace NetTodoApi.Models;

public class TodoItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; }
    public bool IsCompleted { get; set; }

    public Guid UserId { get; set; }
}