using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Номер документа")]
        public string Number { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата поступления")]
        public DateTime Date { get; set; } = DateTime.Today;

        public bool IsActive { get; set; } 

        public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
    }
}