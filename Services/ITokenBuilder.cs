namespace Services
{
    public interface ITokenBuilder
    {
        string BuildToken(string email);
    }
}