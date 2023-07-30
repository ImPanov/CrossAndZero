using CrossZero.Shared;
using Orleans;

namespace CrossZero.Interfaces;

public interface IGameEngine : IGrainWithGuidKey
{
    Task<GameStatus> AddPlayerToGame(int idPlayer);
    Task<GameStatus> GetState();
    Task<List<GameMove>> GetMoves();
    Task<GameStatus> MakeMove(GameMove move);
    Task<GameSummary> GetGameSummary(int idPlayer);
    Task SetName(string name);
}
