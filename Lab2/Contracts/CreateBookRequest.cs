namespace MyBooks.Contracts;

public record CreateBookRequest(string Title, string Description, string Author, string PublicationDate);
