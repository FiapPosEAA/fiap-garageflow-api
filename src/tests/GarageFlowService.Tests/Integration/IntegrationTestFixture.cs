using GarageFlowService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GarageFlowService.Tests.Integration;

public class IntegrationTestFixture : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    public AppDbContext DbContext { get; private set; }

    public IntegrationTestFixture()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        DbContext = new AppDbContext(_options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext?.Database.EnsureDeleted();
        DbContext?.Dispose();
    }

    public void ResetDatabase()
    {
        DbContext?.Database.EnsureDeleted();
        DbContext = new AppDbContext(_options);
        DbContext.Database.EnsureCreated();
    }
}
