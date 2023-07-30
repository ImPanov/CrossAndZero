using CrossZero.Shared;
using CrossZero.Interfaces;
using Orleans;
using Orleans.Concurrency;

namespace CrossZero.Implementations;

[Reentrant]
public class GameEngine : Grain, IGameEngine
{
    public List<int> PlayerIds = new();
    private int _indexNextPlayerToMove;

    private GameStatus _gameStatus;
    private int _winnerId;
    private int _loserId;

    private List<GameMove> _moves = new();
    private int[,] _board = null;

    private string _name = null;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        PlayerIds = new List<int>();
        _moves = new List<GameMove>();
        _indexNextPlayerToMove = -1;
        _board = new int[3, 3]
        {
            { -1, -1, -1 },
            { -1, -1, -1 },
            { -1, -1, -1 },
        };
        _gameStatus = GameStatus.AwaitingPlayer;
        _winnerId = 0;
        _loserId = 0;

        return base.OnActivateAsync(cancellationToken);
    }

    public Task<GameStatus> AddPlayerToGame(int idPlayer)
    {
        if (_gameStatus == GameStatus.Finish) throw new ApplicationException("Can't join game, it's over");
        if (_gameStatus == GameStatus.Playing) throw new ApplicationException("Can't join game, because it's already playing");

        PlayerIds.Add(idPlayer);

        if (_gameStatus is GameStatus.AwaitingPlayer && PlayerIds.Count is 2)
        {
            _gameStatus = GameStatus.Playing;
            _indexNextPlayerToMove = Random.Shared.Next(0,1);
        }

        return Task.FromResult(_gameStatus);
    }

    public async Task<GameSummary> GetGameSummary(int idPlayer)
    {
        var promises = new List<Task<string>>();

        foreach(var p in PlayerIds.Where(p=>p != idPlayer))
        {
            promises.Add(GrainFactory.GetGrain<IPlayer>(p).GetUserName());
        }
        await Task.WhenAll(promises);
        return new GameSummary
        {
            NumMoves = _moves.Count,
            Status = _gameStatus,
            YourMove = _gameStatus is GameStatus.Playing && idPlayer == PlayerIds[_indexNextPlayerToMove],
            NumPlayers = PlayerIds.Count,
            GameId = this.GetPrimaryKey(),
            Usernames = promises.Select(p => p.Result).ToArray(),
            Name = _name,
            GameStarter = PlayerIds.FirstOrDefault() == idPlayer,
        };
    }

    public Task<List<GameMove>> GetMoves() => Task.FromResult(_moves);

    public Task<GameStatus> GetState() => Task.FromResult(_gameStatus);

    public async Task<GameStatus> MakeMove(GameMove move)
    {
        if (_gameStatus is not GameStatus.Playing) throw new ApplicationException("This game is not playing");

        if (PlayerIds.IndexOf(move.PlayerId) < 0) throw new ArgumentException("No such playerId for this game", "move");
        if(move.PlayerId != PlayerIds[_indexNextPlayerToMove]) throw new ArgumentException("The wrong player tried to make a move", "move");

        if (move.X < 0 || move.X > 2 || move.Y < 0 || move.Y > 2) throw new ArgumentException("Bad co-ordinates for a move", "move");
        if (_board[move.X, move.Y] != -1) throw new ArgumentException("That square is not empty", "move");

        _moves.Add(move);
        _board[move.X, move.Y] = _indexNextPlayerToMove;

        var win = false;
        for (var i = 0; i < 3 && !win; i++)
        {
            win = IsWinningLine(_board[i, 0], _board[i, 1], _board[i, 2]);
        }

        if (!win)
        {
            for (var i = 0; i < 3 && !win; i++)
            {
                win = IsWinningLine(_board[0, i], _board[1, i], _board[2, i]);
            }
        }

        if (!win)
        {
            win = IsWinningLine(_board[0, 0], _board[1, 1], _board[2, 2]);
        }

        if (!win)
        {
            win = IsWinningLine(_board[0, 2], _board[1, 1], _board[2, 0]);
        }

        var draw = false;
        if (_moves.Count is 9)
        {
            draw = true; 
        }

        if (win || draw)
        {
            _gameStatus = GameStatus.Finish;
            if (win)
            {
                _winnerId = PlayerIds[_indexNextPlayerToMove];
                _loserId = PlayerIds[(_indexNextPlayerToMove + 1) % 2];
            }
            var promises = new List<Task>();
            var playerGrain = GrainFactory.GetGrain<IPlayer>(PlayerIds[_indexNextPlayerToMove]);
            promises.Add(playerGrain.LeaveGame(this.GetPrimaryKey(), win ? GameFinish.Win : GameFinish.Draw));

            playerGrain = GrainFactory.GetGrain<IPlayer>(PlayerIds[(_indexNextPlayerToMove + 1) % 2]);
            promises.Add(playerGrain.LeaveGame(this.GetPrimaryKey(), win ? GameFinish.Lose : GameFinish.Draw));
            await Task.WhenAll(promises);
            return _gameStatus;
        }
        _indexNextPlayerToMove = (_indexNextPlayerToMove + 1) % 2;
        return _gameStatus;
    }
    private static bool IsWinningLine(int i, int j, int k) => (i, j, k) switch
    {
        (0, 0, 0) => true,
        (1, 1, 1) => true,
        _ => false
    };
    public Task SetName(string name)
    {
        _name = name;
        return Task.CompletedTask;
    }
    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        return base.OnDeactivateAsync(reason, cancellationToken);   
    }
}
