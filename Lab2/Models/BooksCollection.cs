namespace MyBooks.Models;

public class BooksCollection
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public List<Book> Books { get; set; }

    public BooksCollection(string title, string description)
    {
		Title = title;
		Description = description;
		Books = [];
    }
}
