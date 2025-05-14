namespace Calgon.Host.Controllers.Users.Models;

internal sealed class CreateUserModel
{
    public required string Nickname { get; init; }
}

internal sealed class UserCreatedModel
{
    public required string AccessToken { get; init; }
}