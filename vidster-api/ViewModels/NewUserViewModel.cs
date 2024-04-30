namespace vidster_api.ViewModels;

public class NewUserViewModel
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;
}