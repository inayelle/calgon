using Calgon.Host.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calgon.Host.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public virtual DbSet<Room> Rooms { get; set; } = null!;
    public virtual DbSet<RoomMember> RoomMembers { get; set; } = null!;
}
