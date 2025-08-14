using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;
    }
}