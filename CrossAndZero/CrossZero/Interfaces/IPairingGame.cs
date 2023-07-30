using CrossZero.Shared;
using Orleans;

namespace CrossZero.Interfaces;

public interface IPairingGame : IGrainWithIntegerKey
{
    Task AddGame(Guid gameId, string name);
    Task RemoveGame(Guid gameId);
    Task<PairingSummary[]> GetGames(); 
}
