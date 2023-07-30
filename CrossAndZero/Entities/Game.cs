using System.ComponentModel.DataAnnotations.Schema;

namespace Packet.Shared;

public class Game
{
    public Guid GameId { get; set; }
    public string Name { get; set; }
    public required int Player1 { get; set; }
    public required int Player2 { get; set; }
    public required string Result1 { get; set; }
    public required string Result2 { get; set; }
    public required DateTime DateTimeGame { get; set; }
    [ForeignKey("Player1")]
    public User User1 { get; set; }
    [ForeignKey("Player2")]
    public User User2 { get; set; }
}
