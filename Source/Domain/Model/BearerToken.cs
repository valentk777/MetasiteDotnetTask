namespace MetaApp.Domain.Model;

public class BearerToken
{
    public string? Bearer { get; set; }

    public BearerToken(string? bearer)
    {
        Bearer = bearer;
    }
}
