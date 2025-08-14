using WarehouseManagement.Data;
using WarehouseManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WarehouseManagement.Services
{
    public class WarehouseService
    {
        private readonly WarehouseDbContext _context;

        public WarehouseService(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStockBalance(Receipt receipt, bool isDelete = false)
        {
            foreach (var item in receipt.Items)
            {
                var balance = await _context.StockBalances
                    .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

                if (balance == null && !isDelete)
                {
                    balance = new StockBalance
                    {
                        ResourceId = item.ResourceId,
                        UnitId = item.UnitId,
                        Quantity = item.Quantity
                    };
                    await _context.StockBalances.AddAsync(balance);
                }
                else if (balance != null)
                {
                    balance.Quantity = isDelete
                        ? balance.Quantity - item.Quantity
                        : balance.Quantity + item.Quantity;

                    if (balance.Quantity < 0)
                        throw new InvalidOperationException("Недостаточно ресурсов на складе");
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckStockAvailability(Receipt receipt)
        {
            foreach (var item in receipt.Items)
            {
                var balance = await _context.StockBalances
                    .FirstOrDefaultAsync(b => b.ResourceId == item.ResourceId && b.UnitId == item.UnitId);

                if (balance == null || balance.Quantity < item.Quantity)
                    return false;
            }
            return true;
        }
    }
}