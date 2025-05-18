namespace Calgon.Host.Interfaces;

public interface ICurrentUserService
{
    string? CurrentUserId { get; set; }
}