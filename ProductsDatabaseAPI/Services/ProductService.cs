using System;
using NodaTime;
using ProductDatabaseAPI.Models;
using ProductDatabaseAPI.Dtos;
using ProductDatabaseAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductDatabaseAPI.Services;

public class ProductService
{
	private readonly IClock _clock;
    private readonly ApplicationDbContext _context;

	public ProductService(IClock clock, ApplicationDbContext context)
	{
		_clock = clock;
        _context = context;
	}

    public async Task<Product> Find(int Id)
    {
        var result = await _context.Products.FindAsync(Id)
            ?? throw new KeyNotFoundException("Product with specified ID not found");

        return result;
    }

    public Task<Product[]> FindAll()
        => _context.Products.AsNoTracking().ToArrayAsync();

    public async Task<Product> Create(ProductInputDto input)
    {
        var product = new Product
        {
            Name = input.Name,
            DateCreated = DateOnly.FromDateTime(_clock.GetCurrentInstant().ToDateTimeUtc()),
            Price = input.Price
        };

        await _context.AddAsync(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product> Update(int id, ProductInputDto input)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new KeyNotFoundException("Product with specified ID not found");

        product.Name = input.Name;
        product.Price = input.Price;

        _context.Update(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task Delete(int Id)
    {
        var result = await _context.Products.FindAsync(Id)
            ?? throw new KeyNotFoundException();
        _context.Products.Remove(result);
        await _context.SaveChangesAsync();
    }
}

