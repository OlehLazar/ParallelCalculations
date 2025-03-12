namespace MyBooks.Contracts;

public record BookDto(Guid Id, string Title, string Description, string Author, string PublicationDate);
