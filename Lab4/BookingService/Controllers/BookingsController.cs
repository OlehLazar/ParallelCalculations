﻿using BookingService.Context;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
	private readonly ApplicationDbContext _context;
	private readonly BookingsService _bookingService;

	public BookingsController(ApplicationDbContext context, BookingsService bookingService)
	{
		_context = context;
		_bookingService = bookingService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<BookingRequest>>> GetBookings()
	{
		return await _context.BookingRequests.Include(b => b.Room).Include(b => b.Invoice).ToListAsync();
	}

	[Authorize(Roles = "Client")]
	[HttpPost]
	public async Task<ActionResult<BookingRequest>> CreateBooking(BookingRequest request)
	{
		_context.BookingRequests.Add(request);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetBookings), new { id = request.Id }, request);
	}

	[Authorize(Roles = "Admin")]
	[HttpPut("{id}/assign-room")]
	public async Task<IActionResult> AssignRoom(int id)
	{

		var booking = await _context.BookingRequests.FindAsync(id);
		if (booking == null)
		{
			return NotFound();
		}

		var room = await _bookingService.FindAvailableRoomAsync(booking.RoomClass, booking.NumberOfGuests);
		if (room == null)
		{
			return BadRequest("No available rooms");
		}

		var invoice = await _bookingService.CreateInvoiceAsync(booking, room);

		return Ok(new { Message = "Room assigned and invoice generated", Invoice = invoice });
	}
}
