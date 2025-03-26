namespace Lab3.Models;

public class Invoice
{
	public int Id { get; set; }
	public int BookingRequestId { get; set; }
	public decimal TotalPrice { get; set; }
	public DateTime IssueDate { get; set; } = DateTime.UtcNow;
}
