using System.ComponentModel.DataAnnotations.Schema;
using WarehouseManagement.Models;

public class ReceiptItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Убедитесь, что Id автоинкрементный
    public int Id { get; set; }

    public int ReceiptId { get; set; }
    public Receipt Receipt { get; set; }

    public int ResourceId { get; set; }
    public Resource Resource { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; }

    public decimal Quantity { get; set; }
}