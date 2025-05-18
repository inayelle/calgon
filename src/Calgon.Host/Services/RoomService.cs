using Calgon.Host.Data;

namespace Calgon.Host.Services;

public class RoomService(ApplicationDbContext context, CurrentUserService currentUser)
{
    public void GetRoom()
    {
        Console.WriteLine(currentUser.CurrentUserId);
    }
}