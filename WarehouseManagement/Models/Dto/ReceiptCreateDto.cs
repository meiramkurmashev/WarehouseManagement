// WarehouseManagement.Models/ReceiptCreateDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.Dto
{
    public class ReceiptCreateDto
    {
        public int Id { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsActive { get; set; }

        // Используем ReceiptItemCreateDto вместо ReceiptItemDto
        public List<ReceiptItemCreateDto> Items { get; set; } = new List<ReceiptItemCreateDto>();
    }

    public class ReceiptItemDto
    {
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}