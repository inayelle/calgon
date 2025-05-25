using AnyKit.Pipelines;

namespace Calgon.Game;

internal interface IGamePipe
{
    void Invoke(GameContext context, Pipeline<GameContext> next);
}