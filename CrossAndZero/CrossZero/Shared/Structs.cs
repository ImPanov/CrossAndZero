
namespace CrossZero.Shared;
[GenerateSerializer]
public struct GameMove
{
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
[GenerateSerializer]
public struct GameSummary
{
    public GameStatus Status { get; set; }
    public bool YourMove { get; set; }
    public int NumMoves { get; set; }
    public GameFinish Result { get; set; }
    public int NumPlayers { get; set; }
    public Guid GameId { get; set; }
    public string[] Usernames { get; set; }
    public string Name { get; set; }
    public bool GameStarter { get; set; }
}
