namespace MetaApp.Domain.Models;

public class AuthorizationRequest
{
    public string Username { get; set; }

    public string Password { get; set; }

    public AuthorizationRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
