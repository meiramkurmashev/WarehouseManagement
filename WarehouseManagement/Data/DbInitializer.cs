using WarehouseManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManagement.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(WarehouseDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Resources.AnyAsync())
            {
                await context.Resources.AddRangeAsync(
                    new Resource { Name = "Гвозди", IsActive = true },
                    new Resource { Name = "Доски", IsActive = true },
                    new Resource { Name = "Краска", IsActive = true }
                );
            }

            if (!await context.Units.AnyAsync())
            {
                await context.Units.AddRangeAsync(
                    new Unit { Name = "шт.", IsActive = true },
                    new Unit { Name = "кг", IsActive = true },
                    new Unit { Name = "л", IsActive = true }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}