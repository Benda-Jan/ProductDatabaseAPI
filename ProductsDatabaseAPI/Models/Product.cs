
namespace ProductDatabaseAPI.Models;

public record Product
{
    public int Id { get; set; }

    public required DateOnly DateCreated { get; set; }

    public required string Name { get; set; } = String.Empty;

    public required int Price { get; set; }
}

