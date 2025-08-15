using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Models;
using WarehouseManagement.Services;
using System.Threading.Tasks;
using WarehouseManagement.Data;

namespace WarehouseManagement.Controllers
{
    public class UnitsController : BaseController<Unit>
    {
        public UnitsController(IRepository<Unit> repository, WarehouseDbContext context)
            : base(repository, context)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create([Bind("Id,Name")] Unit unit)
        {
            if (await _repository.ExistsAsync(u => u.Name == unit.Name))
                ModelState.AddModelError("Name", "Единица измерения с таким названием уже существует");

            return await base.Create(unit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsActive")] Unit unit)
        {
            if (await _repository.ExistsAsync(u => u.Name == unit.Name && u.Id != id))
                ModelState.AddModelError("Name", "Единица измерения с таким названием уже существует");

            return await base.Edit(id, unit);
        }

        protected override async Task<bool> IsEntityUsed(int id)
        {
            return await _context.ReceiptItems.AnyAsync(ri => ri.UnitId == id);
        }
    }
}