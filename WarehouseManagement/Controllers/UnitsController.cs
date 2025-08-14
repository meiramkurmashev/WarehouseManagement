using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    public class UnitsController : Controller
    {
        private readonly IRepository<Unit> _repository;

        public UnitsController(IRepository<Unit> repository)
        {
            _repository = repository;
        }

        // GET: Units
        public async Task<IActionResult> Index(bool showInactive = false)
        {
            var units = await _repository.GetAllAsync();

            if (!showInactive)
            {
                units = units.Where(u => u.IsActive).ToList();
            }
            if (showInactive) {
                units = units.Where(u => u.IsActive == false).ToList();
            }
            ViewBag.ShowInactive = showInactive;
            return View(units);
        }

        // GET: Units/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _repository.GetByIdAsync(id.Value);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // GET: Units/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Units/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Unit unit)
        {
            if (await _repository.ExistsAsync(u => u.Name == unit.Name))
            {
                ModelState.AddModelError("Name", "Единица измерения с таким названием уже существует");
            }

            if (ModelState.IsValid)
            {
                unit.IsActive = true;
                await _repository.AddAsync(unit);
                return RedirectToAction(nameof(Index));
            }
            return View(unit);
        }

        // GET: Units/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _repository.GetByIdAsync(id.Value);
            if (unit == null)
            {
                return NotFound();
            }
            return View(unit);
        }

        // POST: Units/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsActive")] Unit unit)
        {
            if (id != unit.Id)
            {
                return NotFound();
            }

            if (await _repository.ExistsAsync(u => u.Name == unit.Name && u.Id != id))
            {
                ModelState.AddModelError("Name", "Единица измерения с таким названием уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(unit);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UnitExists(unit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unit);
        }

        // GET: Units/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _repository.GetByIdAsync(id.Value);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unit = await _repository.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            // Вместо удаления переводим в архив
            unit.IsActive = false;
            await _repository.UpdateAsync(unit);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UnitExists(int id)
        {
            return await _repository.GetByIdAsync(id) != null;
        }
    }
}