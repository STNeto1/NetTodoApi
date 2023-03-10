namespace NetTodoApi.DTO.Auth;

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
}