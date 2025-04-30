using System.ComponentModel.DataAnnotations;

namespace BookingService.Models;

public class BookingRequest
{
	public int Id { get; set; }

	[Required]
	public string CustomerName { get; set; }

	[Required]
	public int NumberOfGuests { get; set; }

	[Required]
	public string RoomClass { get; set; }

	[Required]
	public int DurationDays { get; set; }

	public int? RoomId { get; set; }
	public Room Room { get; set; }

	public Invoice Invoice { get; set; }
}
