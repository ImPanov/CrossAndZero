using CrossZero.Interfaces;
using CrossZero.Shared;
using Microsoft.Extensions.Caching.Memory;
using Orleans;
using Orleans.Concurrency;

namespace CrossZero.Implementations;

[Reentrant]
public class PairingGame : Grain, IPairingGame
{
    private readonly HashSet<PairingSummary> _cache = new();
    public Task AddGame(Guid gameId, string name)
    {
        _cache.Add(new PairingSummary { Name = name, GameId = gameId });
        return Task.CompletedTask;
    }

    public Task<PairingSummary[]> GetGames()
    {
        return Task.FromResult(_cache.Select(x => new PairingSummary { GameId = x.GameId, Name = x.Name }).ToArray());
    }

    public Task RemoveGame(Guid gameId)
    {
        _cache.RemoveWhere(c => c.GameId == gameId);
        return Task.CompletedTask;
    }
}
