namespace Packet.Shared
{
    public class User
    {
        public int UserId { get; set; }
        public required string userName { get; set; }
        public required string Password { get; set; }
    }
}
