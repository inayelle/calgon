using Calgon.Host.Interfaces;

namespace Calgon.Host.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? CurrentUserId { get; set; }
}