using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: base(options) { }

	public DbSet<BookingRequest> BookingRequests { get; set; }
	public DbSet<Room> Rooms { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
}
