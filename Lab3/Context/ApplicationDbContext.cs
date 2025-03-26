using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: base(options) { }

	public DbSet<User> Users { get; set; }
	public DbSet<BookingRequest> BookingRequests { get; set; }
	public DbSet<Room> Rooms { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
}