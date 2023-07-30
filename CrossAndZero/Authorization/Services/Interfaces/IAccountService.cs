using Packet.Shared;

namespace BasicAuthorization.Services.Interfaces;

public interface IAccountService
{
    //string CreateHashPassword(string password);
    Task<User> AuthenticateAsync(string name, string password);
    Task<User> RegisterAsync(string name, string password);

}
