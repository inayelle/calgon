namespace Calgon.Host.Game.Client.Args;

public sealed class PlayerEliminatedArgs
{
    public required Guid PlayerId { get; init; }
}