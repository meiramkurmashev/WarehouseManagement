using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class StockBalance
    {
        public int Id { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}