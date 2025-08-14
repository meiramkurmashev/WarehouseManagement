public class ReceiptFilterModel
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int[] DocumentNumbers { get; set; }
    public int[] ResourceIds { get; set; }
    public int[] UnitIds { get; set; }
}