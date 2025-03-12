namespace MyBooks.Models;

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public List<BooksCollection> BooksCollections { get; set; }

	public User(string name)
	{
		Name = name;
		BooksCollections = [];
	}
}
