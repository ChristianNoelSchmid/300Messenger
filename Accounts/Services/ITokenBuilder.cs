namespace _300Messenger.Accounts.Services
{
    public interface ITokenBuilder
    {
        string BuildToken(string email);
    }
}