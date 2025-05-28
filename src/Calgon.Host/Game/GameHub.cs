using Calgon.Host.Game.Client;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Game;

public sealed class GameHub : Hub<IGameHubClient>;