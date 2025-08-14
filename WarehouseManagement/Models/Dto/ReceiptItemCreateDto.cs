using System.ComponentModel.DataAnnotations;

public class ReceiptItemCreateDto
{
    [Required(ErrorMessage = "Выберите ресурс")]
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "Выберите единицу измерения")]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Укажите количество")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
    public decimal Quantity { get; set; }

    public int Id { get; set; } // Добавляем Id для поддержки редактирования существующих элементов
}
