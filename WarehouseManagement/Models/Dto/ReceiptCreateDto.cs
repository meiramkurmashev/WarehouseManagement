// WarehouseManagement.Models/ReceiptCreateDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.Dto
{
    public class ReceiptCreateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле номера обязательно")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Номер должен содержать только цифры")]
        [Display(Name = "Номер документа")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        public bool IsActive { get; set; }

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