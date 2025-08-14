using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;
    }
}