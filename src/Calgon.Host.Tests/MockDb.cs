

using Calgon.Host.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Calgon.Host.Tests;

public class MockDb
{
    public ApplicationDbContext Context { get; }
    public MockDb()
    {
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Calgon4ik")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new ApplicationDbContext(contextOptions);

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
}
