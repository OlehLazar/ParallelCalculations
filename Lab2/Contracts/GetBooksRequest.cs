namespace MyBooks.Contracts;

public record GetBooksRequest(string? Search, string? SortItem, string? SortOrder);
