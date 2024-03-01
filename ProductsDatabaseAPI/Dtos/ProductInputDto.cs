using System;

namespace ProductDatabaseAPI.Dtos;

public class ProductInputDto
{
	public required string Name { get; set;}

    public required int Price { get; set; }
}

