
namespace Calgon.Game.Tests.Game;
public class DefaultMapGeneratorTests
{
    private DefaultMapGenerator _generator;

    [SetUp]
    public void Setup()
    {
        _generator = new DefaultMapGenerator();
    }

    [Test]
    public void Generate_ReturnsMapWithPlanetsWithinBounds()
    {
        var map = _generator.Generate();

        Assert.That(map.Size, Is.EqualTo(300));
        foreach (var planet in map.Planets)
        {
            Assert.That(planet.Location.X, Is.GreaterThanOrEqualTo(planet.Size));
            Assert.That(planet.Location.X, Is.LessThanOrEqualTo(map.Size - planet.Size));
            Assert.That(planet.Location.Y, Is.GreaterThanOrEqualTo(planet.Size));
            Assert.That(planet.Location.Y, Is.LessThanOrEqualTo(map.Size - planet.Size));
        }
    }

    [Test]
    public void Generate_ReturnsUniquePlanetsWithSufficientDistance()
    {
        var map = _generator.Generate();

        foreach (var p1 in map.Planets)
        {
            foreach (var p2 in map.Planets)
            {
                if (p1 == p2) continue;
                double minDistance = 10 + p1.Size + p2.Size;
                Assert.That(p1.Location.DistanceTo(p2.Location), Is.GreaterThanOrEqualTo(minDistance));
            }
        }
    }

    [Test]
    public void Generate_ReturnsPlanetsCountUpToCapacity()
    {
        var map = _generator.Generate();
        Assert.That(map.Planets.Count, Is.LessThanOrEqualTo(18));
    }

    [Test]
    public void Generate_PlanetsHaveSizesWithinRange()
    {
        var map = _generator.Generate();

        foreach (var planet in map.Planets)
        {
            Assert.That(planet.Size, Is.InRange(10, 15));
        }
    }
}
