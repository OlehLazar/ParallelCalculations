namespace BookingService.Models;

public class Room
{
	public int Id { get; set; }
	public string RoomClass { get; set; }
	public int Capacity { get; set; }
	public bool IsAvailable { get; set; } = true;
}
