using Microsoft.EntityFrameworkCore;
using MyBooks.Models;

namespace MyBooks.DataAccess;

public class BooksDbContext : DbContext
{
	public DbSet<Book> Books => Set<Book>();
	private readonly IConfiguration _configuration;

	public BooksDbContext(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite(_configuration.GetConnectionString("Database"));
	}
}
