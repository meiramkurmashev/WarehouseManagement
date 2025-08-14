using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Services;

public class ResourcesController : Controller
{
    private readonly IRepository<Resource> _repository;
    private readonly WarehouseDbContext _context;

    public ResourcesController(IRepository<Resource> repository, WarehouseDbContext context)
    {
        _repository = repository;
        _context = context;
    }
    public async Task<IActionResult> Index(bool showInactive = false)
    {
        var resources = await _repository.GetAllAsync();
        if (!showInactive) { 
            resources = resources.Where(r => r.IsActive).ToList();
        }
        else { 
            resources = resources.Where(u => u.IsActive == false);
        }

        return View(resources);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Resource resource)
    {
        if (await _repository.ExistsAsync(r => r.Name == resource.Name))
            ModelState.AddModelError("Name", "Ресурс с таким названием уже существует");

        if (ModelState.IsValid)
        {
            await _repository.AddAsync(resource);
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _repository.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Resource resource)
    {
        if (id != resource.Id) return NotFound();

        if (await _repository.ExistsAsync(r => r.Name == resource.Name && r.Id != id))
            ModelState.AddModelError("Name", "Ресурс с таким названием уже существует");

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(resource);
            return RedirectToAction(nameof(Index));
        }
        return View(resource);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var resource = await _repository.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resource = await _repository.GetByIdAsync(id);
        if (resource == null) return NotFound();

        resource.IsActive = false;
        await _repository.UpdateAsync(resource);

        return RedirectToAction(nameof(Index));
    }
    private async Task<bool> IsResourceUsed(int resourceId)
    {
        return await _context.ReceiptItems.AnyAsync(ri => ri.ResourceId == resourceId);
    }
    [HttpPost, ActionName("HardDelete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDeleteConfirmed(int id)
    {
        var resource = await _repository.GetByIdAsync(id);
        if (resource == null)
        {
            return NotFound();
        }

        if (await IsResourceUsed(id))
        {
            TempData["ErrorMessage"] = "Ресурс нельзя удалить, так как он используется в документах поступления!";
            return RedirectToAction(nameof(Edit), new { id });
        }

        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Ресурс успешно удалён!";
        return RedirectToAction(nameof(Index));
    }
}