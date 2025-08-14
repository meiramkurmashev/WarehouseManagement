using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models.Dto;

namespace WarehouseManagement.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ReceiptsController(WarehouseDbContext context)
        {
            _context = context;
        }

        // GET: Receipts
        [HttpGet]
        public async Task<IActionResult> Index(
     bool showInactive = false,
     DateTime? fromDate = null,
     DateTime? toDate = null,
     int[] resourceIds = null,
     int[] unitIds = null)
        {
            var query = _context.Receipts
                .Include(r => r.Items)
                    .ThenInclude(i => i.Resource)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Unit)
                .AsQueryable();

            // Фильтрация
            if (!showInactive)
            {
                query = query.Where(r => r.IsActive);
            }

            if (fromDate.HasValue)
            {
                var fromDateUtc = fromDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
                    : fromDate.Value.ToUniversalTime();
                query = query.Where(r => r.Date >= fromDateUtc);
            }

            if (toDate.HasValue)
            {
                var toDateUtc = toDate.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(toDate.Value.AddDays(1).Date, DateTimeKind.Utc)
                    : toDate.Value.ToUniversalTime().AddDays(1).Date;
                query = query.Where(r => r.Date < toDateUtc);
            }

            if (resourceIds != null && resourceIds.Length > 0)
            {
                query = query.Where(r => r.Items.Any(i => resourceIds.Contains(i.ResourceId)));
            }

            if (unitIds != null && unitIds.Length > 0)
            {
                query = query.Where(r => r.Items.Any(i => unitIds.Contains(i.UnitId)));
            }

            // Данные для фильтров
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsActive)
                .ToListAsync();

            ViewBag.Units = await _context.Units
                .Where(u => u.IsActive)
                .ToListAsync();

            return View(await query.ToListAsync());
        }

        // GET: Receipts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts
                .Include(r => r.Items)
                    .ThenInclude(i => i.Resource)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Unit)
                .FirstOrDefaultAsync(m => m.Id == id    );

            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

  
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadViewData();
            var model = new ReceiptCreateDto
            {
                Date = DateTime.Today,
                Items = new List<ReceiptItemCreateDto> { new ReceiptItemCreateDto() } // Добавляем один пустой элемент
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptCreateDto receiptDto)
        {
            // Проверка уникальности номера
            if (await _context.Receipts.AnyAsync(r => r.Number == receiptDto.Number))
            {
                ModelState.AddModelError("Number", "Документ с таким номером уже существует");
                await LoadViewData();
                return View(receiptDto);
            }

            // Фильтрация элементов
            receiptDto.Items = receiptDto.Items?
                .Where(i => i.ResourceId > 0 && i.UnitId > 0 && i.Quantity > 0)
                .ToList();

            if (receiptDto.Items == null || !receiptDto.Items.Any())
            {
                ModelState.AddModelError("", "Добавьте хотя бы один ресурс");
                await LoadViewData();
                return View(receiptDto);
            }

            if (ModelState.IsValid)
            {
                var utcDate = receiptDto.Date.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(receiptDto.Date, DateTimeKind.Utc)
                    : receiptDto.Date.ToUniversalTime();

                var receipt = new Receipt
                {
                    Number = receiptDto.Number,
                    Date = utcDate,
                    IsActive = true,
                    Items = receiptDto.Items.Select(i => new ReceiptItem
                    {
                        ResourceId = i.ResourceId,
                        UnitId = i.UnitId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                _context.Add(receipt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadViewData();
            return View(receiptDto);
        }
        // GET: Receipts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
            {
                return NotFound();
            }

            // Маппинг сущности на DTO
            var receiptDto = new ReceiptCreateDto
            {
                Number = receipt.Number,
                Date = receipt.Date,
                IsActive = receipt.IsActive,
                Items = receipt.Items.Select(i => new ReceiptItemCreateDto
                {
                    Id = i.Id,
                    ResourceId = i.ResourceId,
                    UnitId = i.UnitId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await LoadViewData();
            return View(receiptDto);
        }

        // POST: Receipts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReceiptCreateDto receiptDto)
        {
            if (id != receiptDto.Id)
                return NotFound();

            var receipt = await _context.Receipts
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                return NotFound();

            // Преобразуем дату в UTC
            var utcDate = receiptDto.Date.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(receiptDto.Date, DateTimeKind.Utc)
                : receiptDto.Date.ToUniversalTime();

            // Обновляем основные поля Receipt
            receipt.Number = receiptDto.Number;
            receipt.Date = utcDate; // Используем преобразованную дату
            receipt.IsActive = receiptDto.IsActive;

            // Остальной код остается без изменений
            var itemsToRemove = receipt.Items
                .Where(dbItem => !receiptDto.Items.Any(dtoItem => dtoItem.Id == dbItem.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                if (item.Id > 0)
                {
                    _context.ReceiptItems.Remove(item);
                }
            }

            foreach (var itemDto in receiptDto.Items)
            {
                var existingItem = receipt.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (existingItem != null)
                {
                    existingItem.ResourceId = itemDto.ResourceId;
                    existingItem.UnitId = itemDto.UnitId;
                    existingItem.Quantity = itemDto.Quantity;
                }
                else
                {
                    receipt.Items.Add(new ReceiptItem
                    {
                        ResourceId = itemDto.ResourceId,
                        UnitId = itemDto.UnitId,
                        Quantity = itemDto.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt != null)
            {
                receipt.IsActive = false;
                _context.Receipts.Update(receipt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipts.Any(e => e.Id == id);
        }

        private async Task LoadViewData()
        {
            ViewData["Resources"] = await _context.Resources.ToListAsync();
            ViewData["Units"] = await _context.Units.ToListAsync();
            // Добавьте другие необходимые данные
        }
    }
}