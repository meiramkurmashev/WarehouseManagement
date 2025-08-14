using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WarehouseManagement.Data;

public class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    public WarehouseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=WarehouseDB;Username=postgres;Password=123;Timeout=300");
        
        
        return new WarehouseDbContext(optionsBuilder.Options);
    }
}