using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WarehouseManagement.Data;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    public abstract class BaseController<T> : Controller where T : class
    {
        protected readonly IRepository<T> _repository;
        protected readonly WarehouseDbContext _context;

        public BaseController(IRepository<T> repository, WarehouseDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public virtual async Task<IActionResult> Index(bool showInactive = false)
        {
            var entities = await _repository.GetAllAsync();
            entities = entities.Where(e => showInactive ? !((dynamic)e).IsActive : ((dynamic)e).IsActive).ToList();
            ViewBag.ShowInactive = showInactive;
            return View(entities);
        }

        public virtual async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _repository.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();

            return View(entity);
        }

        public virtual IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(T entity)
        {
            if (ModelState.IsValid)
            {
                ((dynamic)entity).IsActive = true;
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public virtual async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _repository.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(int id, T entity)
        {
            if (id != (int)((dynamic)entity).Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(entity);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EntityExists(id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public virtual async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entity = await _repository.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            ((dynamic)entity).IsActive = false;
            await _repository.UpdateAsync(entity);

            return RedirectToAction(nameof(Index));
        }

        protected virtual async Task<bool> EntityExists(int id)
        {
            return await _repository.GetByIdAsync(id) != null;
        }

        protected virtual async Task<bool> IsEntityUsed(int id)
        {
            // Базовый метод, должен быть переопределен в дочерних классах
            return await Task.FromResult(false);
        }

        [HttpPost, ActionName("HardDelete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> HardDeleteConfirmed(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (await IsEntityUsed(id))
            {
                TempData["ErrorMessage"] = "Сущность нельзя удалить, так как она используется в других документах!";
                return RedirectToAction(nameof(Edit), new { id });
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Сущность успешно удалена!";
            return RedirectToAction(nameof(Index));
        }
    }
}