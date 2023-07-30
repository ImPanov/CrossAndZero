using CrossZero.Interfaces;
using CrossZero.Shared;
using Orleans;

namespace CrossZero.Implementations;

public class Player : Grain, IPlayer
{
    private List<Guid> _activeGames = new();
    private List<Guid> _pastGames = new();
    private int _wins;
    private int _loses;
    private int _gamesStarted;
    private int idPlayer { get; set; }
    private string _username = null!;

    public override Task OnActivateAsync(CancellationToken token)
    {
        _activeGames = new List<Guid>();
        _pastGames = new List<Guid>();
        _wins = 0;
        _loses = 0;
        _gamesStarted = 0;

        return base.OnActivateAsync(token);
    }

    public async Task<PairingSummary[]> GetAvailableGames()
    {
        var grain = GrainFactory.GetGrain<IPairingGame>(0);
        return (await grain.GetGames()).Where(x => !_activeGames.Contains(x.GameId)).ToArray();
    }

    public async Task<Guid> CreateGame()
    {
        _gamesStarted++;

        var gameId = Guid.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameEngine>(gameId);

        await gameGrain.AddPlayerToGame(idPlayer);
        _activeGames.Add(gameId);
        var name = $"{_username}'s {AddOrdinalSuffix(_gamesStarted.ToString())} game";
        await gameGrain.SetName(name);

        var pairingGrain = GrainFactory.GetGrain<IPairingGame>(0);
        await pairingGrain.AddGame(gameId, name);

        return gameId;
    }

    public async Task<GameStatus> JoinGame(Guid gameId)
    {
        var gameGrain = GrainFactory.GetGrain<IGameEngine>(gameId);

        var state = await gameGrain.AddPlayerToGame(idPlayer);
        _activeGames.Add(gameId);

        var pairingGrain = GrainFactory.GetGrain<IPairingGame>(0);
        await pairingGrain.RemoveGame(gameId);

        return state;
    }

    public Task LeaveGame(Guid gameId, GameFinish outcome)
    {
        _activeGames.Remove(gameId);
        _pastGames.Add(gameId);

        _ = outcome switch
        {
            GameFinish.Win => _wins++,
            GameFinish.Lose => _loses++,
            _ => 0
        };

        return Task.CompletedTask;
    }

    public async Task<List<GameSummary>> GetGameSummaries()
    {
        var tasks = new List<Task<GameSummary>>();
        foreach (var gameId in _activeGames)
        {
            var game = GrainFactory.GetGrain<IGameEngine>(gameId);
            tasks.Add(game.GetGameSummary(idPlayer));
        }

        await Task.WhenAll(tasks);
        return tasks.Select(x => x.Result).ToList();
    }

    public Task SetUsername(string name)
    {
        _username = name;
        return Task.CompletedTask;
    }

    public Task<string> GetUserName() => Task.FromResult(_username);

    private static string AddOrdinalSuffix(string number)
    {
        var n = int.Parse(number);
        var nMod100 = n % 100;

        return nMod100 switch
        {
            >= 11 and <= 13 => string.Concat(number, "th"),
            _ => (n % 10) switch
            {
                1 => string.Concat(number, "st"),
                2 => string.Concat(number, "nd"),
                3 => string.Concat(number, "rd"),
                _ => string.Concat(number, "th"),
            }
        };
    }
}