using CrossZero.Shared;
using Orleans;

namespace CrossZero.Interfaces;

public interface IPlayer : IGrainWithIntegerKey
{
    Task<PairingSummary[]> GetAvailableGames();

    Task<List<GameSummary>> GetGameSummaries();

    Task<Guid> CreateGame();

    Task<GameStatus> JoinGame(Guid gameId);

    Task LeaveGame(Guid gameId, GameFinish Result);
    Task<string> GetUserName();
}
