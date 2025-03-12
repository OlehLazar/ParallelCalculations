namespace MyBooks.Models;

public class Book
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string Author { get; set; }
	public string PublicationDate { get; set; }

	public Book(string title, string description, string author, string publicationDate)
	{
		Title = title;
		Description = description;
		Author = author;
		PublicationDate = publicationDate;
	}
}
