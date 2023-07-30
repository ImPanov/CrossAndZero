namespace CrossZero.Shared;
[Serializable]
public enum GameStatus
{
    AwaitingPlayer,
    Playing,
    Finish
}
[Serializable]
public enum GameFinish
{
    Win,
    Lose,
    Draw
}