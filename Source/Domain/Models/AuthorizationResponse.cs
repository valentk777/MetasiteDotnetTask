namespace MetaApp.Domain.Models;

public class AuthorizationResponse
{
    public string? Bearer { get; set; }

    public AuthorizationResponse(string? bearer)
    {
        Bearer = bearer;
    }
}
