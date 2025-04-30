using BookingService.Context;
using BookingService.Models;

namespace BookingService.Services;

public class BookingsService
{
	private readonly ApplicationDbContext _context;

	public BookingsService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<Room> FindAvailableRoomAsync(string roomClass, int guests)
	{
		return _context.Rooms
			.FirstOrDefault(r => r.RoomClass == roomClass && r.Capacity >= guests && r.IsAvailable);
	}

	public async Task<Invoice> CreateInvoiceAsync(BookingRequest request, Room room)
	{
		var pricePerDay = room.RoomClass switch
		{
			"Економ" => 50m,
			"Стандарт" => 100m,
			"Люкс" => 200m,
			_ => 100m
		};

		var invoice = new Invoice
		{
			BookingRequestId = request.Id,
			TotalPrice = pricePerDay * request.DurationDays
		};

		request.RoomId = room.Id;
		room.IsAvailable = false;

		_context.Invoices.Add(invoice);
		await _context.SaveChangesAsync();

		return invoice;
	}
}
