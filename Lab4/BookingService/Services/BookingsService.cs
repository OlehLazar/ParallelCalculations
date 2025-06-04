using BookingService.Context;
using BookingService.Models;
using Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace BookingService.Services
{
	public class BookingsService
	{
		private readonly IDistributedCache _cache;
		private readonly ApplicationDbContext _context;

		public BookingsService(IDistributedCache cache, ApplicationDbContext context)
		{
			_cache = cache;
			_context = context;
		}

		public async Task<Room> FindAvailableRoomAsync(string roomClass, int guests)
		{
			string cacheKey = $"AvailableRoom_{roomClass}_{guests}";
			var roomFromCache = await _cache.GetRecordAsync<Room>(cacheKey);

			if (roomFromCache != null)
			{
				return roomFromCache;
			}

			var room = _context.Rooms
				.FirstOrDefault(r => r.RoomClass == roomClass && r.Capacity >= guests && r.IsAvailable);

			if (room != null)
			{
				await _cache.SetRecordAsync(room, cacheKey, TimeSpan.FromMinutes(5));
			}

			return room;
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

			// invalidate room cache after booking
			string cacheKey = $"AvailableRoom_{room.RoomClass}_{room.Capacity}";
			await _cache.RemoveAsync(cacheKey);

			return invoice;
		}
	}
}
