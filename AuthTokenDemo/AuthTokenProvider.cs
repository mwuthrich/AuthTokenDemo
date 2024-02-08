public interface IAuthTokenProvider
{
    Task<string> GetToken();
}

public class AuthTokenProvider : IAuthTokenProvider
{
    public Task<string> GetToken()
    {
        return Task.FromResult("password!");
    }
}