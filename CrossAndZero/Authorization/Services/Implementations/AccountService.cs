using BasicAuthorization.Services.Interfaces;
using Packet.Shared;
using System.Text;

namespace BasicAuthorization.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext db;
        public AccountService(ApplicationDbContext injectContext)
        {
            db=injectContext;
        }
        public async Task<User> AuthenticateAsync(string name, string password)
        {
            var user = db.Users.SingleOrDefault(p => p.userName == name && p.Password==password);
            if (user == null)
            {
                return user;
            }            
            return user;
            
        }

        public async Task<User> RegisterAsync(string name, string password)
        {
            var user = new User { userName = name, Password = password };
            await db.Users.AddAsync(user);
            db.SaveChanges();
            return user;
        }
    }
}
