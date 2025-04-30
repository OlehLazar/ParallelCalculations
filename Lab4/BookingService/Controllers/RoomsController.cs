using BookingService.Context;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers;


[Route("api/rooms")]
[ApiController]
public class RoomsController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public RoomsController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
	{
		return await _context.Rooms.ToListAsync();
	}

	[HttpPost]
	public async Task<ActionResult<Room>> AddRoom(Room room)
	{
		_context.Rooms.Add(room);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetRooms), new { id = room.Id }, room);
	}
}
