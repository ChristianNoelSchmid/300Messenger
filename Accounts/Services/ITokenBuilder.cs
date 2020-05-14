namespace _300Messenger.Authentication.Services
{
    public interface ITokenBuilder
    {
        string BuildToken(string email);
    }
}