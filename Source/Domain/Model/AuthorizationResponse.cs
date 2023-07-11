namespace MetaApp.Domain.Model;

public class AuthorizationResponse
{
    public string? Bearer { get; set; }

    public AuthorizationResponse(string? bearer)
    {
        Bearer = bearer;
    }
}
