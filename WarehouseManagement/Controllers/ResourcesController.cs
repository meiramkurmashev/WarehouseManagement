using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Models;
using WarehouseManagement.Services;
using System.Threading.Tasks;
using WarehouseManagement.Data;

namespace WarehouseManagement.Controllers
{
    public class ResourcesController : BaseController<Resource>
    {
        public ResourcesController(IRepository<Resource> repository, WarehouseDbContext context)
            : base(repository, context)
        {
        }

        [HttpPost]
        public override async Task<IActionResult> Create(Resource resource)
        {
            try
            {
                if (await _repository.ExistsAsync(r => r.Name == resource.Name))
                    ModelState.AddModelError("Name", "Ресурс с таким названием уже существует");

                return await base.Create(resource);
            }
            catch
            {
                ModelState.AddModelError("", "Произошла ошибка при создании ресурса");
                return View(resource);
            }
        }

        [HttpPost]
        public override async Task<IActionResult> Edit(int id, Resource resource)
        {
            if (await _repository.ExistsAsync(r => r.Name == resource.Name && r.Id != id))
                ModelState.AddModelError("Name", "Ресурс с таким названием уже существует");

            return await base.Edit(id, resource);
        }

        protected override async Task<bool> IsEntityUsed(int id)
        {
            return await _context.ReceiptItems.AnyAsync(ri => ri.ResourceId == id);
        }
    }
}