using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBooks.Contracts;
using MyBooks.DataAccess;
using MyBooks.Models;
using System.Linq.Expressions;

namespace MyBooks.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksDbContext _dbContext;

    public BooksController(BooksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var book = new Book(request.Title, request.Description, request.Author, request.PublicationDate);

        await _dbContext.Books.AddAsync(book, ct);
        await _dbContext.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetBooksRequest request, CancellationToken ct)
    {
        var booksQuery = _dbContext.Books
            .Where(b => string.IsNullOrWhiteSpace(request.Search) ||
                        b.Title.ToLower().Contains(request.Search.ToLower()));

        Expression<Func<Book, object>> selectorKey = request.SortItem?.ToLower() switch
        {
            "title" => book => book.Title,
            "author" => book => book.Author,
            "date" => book => book.PublicationDate,
            _ => book => book.Id,
        };

        booksQuery = request.SortOrder == "desc"
            ? booksQuery.OrderByDescending(selectorKey)
            : booksQuery.OrderBy(selectorKey);

        var bookDtos = await booksQuery
            .Select(b => new BookDto(b.Id, b.Title, b.Description, b.Author, b.PublicationDate))
            .ToListAsync(cancellationToken: ct);

        return Ok(new GetBooksResponse(bookDtos));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateBookRequest request, CancellationToken ct)
    {
        var book = await _dbContext.Books.FindAsync([id], ct);
        if (book == null)
        {
            return NotFound();
        }

        book.Title = request.Title;
        book.Description = request.Description;
        book.Author = request.Author;
        book.PublicationDate = request.PublicationDate;

        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var book = await _dbContext.Books.FindAsync([id], ct);
        if (book == null)
        {
            return NotFound();
        }

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync(ct);

        return NoContent();
    }
}
