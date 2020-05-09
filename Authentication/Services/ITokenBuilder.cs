namespace _300Messenger.Authentication.Services
{
    public interface ITokenBuilder
    {
        string BuildToken(string first, string last, string email);
    }
}