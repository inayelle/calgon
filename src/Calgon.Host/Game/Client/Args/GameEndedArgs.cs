namespace Calgon.Host.Game.Client.Args;

public sealed class GameEndedArgs
{
    public required Guid WinnerId { get; init; }
}