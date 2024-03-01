using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductDatabaseAPI.Data;
using ProductDatabaseAPI.Models;
using ProductDatabaseAPI.Dtos;
using ProductDatabaseAPI.Services;
using NodaTime;
using NodaTime.Testing;

namespace ProductDatabaseAPI.Test;

public class ProductServiceTest
{
    [Fact]
    public async Task Find_Correct()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        await context.Products.AddAsync(new Product
        {
            Id = 1,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        });
        await context.SaveChangesAsync();

        var sut = new ProductService(currentTime, context);

        var result = await sut.Find(1);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task Find_Correct_Empty_DB()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        var sut = new ProductService(currentTime, context);

        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Find(1));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Find_Incorrect()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        await context.Products.AddAsync(new Product
        {
            Id = 1,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        });
        await context.SaveChangesAsync();

        var sut = new ProductService(currentTime, context);

        var result = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Find(2));
        Assert.NotNull(result);
    }

    [Fact]
    public async Task FindAll_Correct()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        await context.Products.AddAsync(new Product
        {
            Id = 1,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        });
        await context.Products.AddAsync(new Product
        {
            Id = 2,
            DateCreated = new DateOnly(2024, 2, 26),
            Name = "Tablet",
            Price = 124
        });
        await context.SaveChangesAsync();

        var sut = new ProductService(currentTime, context);

        var result = await sut.FindAll();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task FindAll_Correct_Empty_DB()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        var sut = new ProductService(currentTime, context);

        var result = await sut.FindAll();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Create_Correct()
    {
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        var productDto = new ProductInputDto
        {
            Name = "Laptop",
            Price = 124
        };

        var sut = new ProductService(currentTime, context);
        var result = await sut.Create(productDto);

        Assert.NotNull(result);
        Assert.NotEmpty(context.Products);
    }

    [Fact]
    public async Task Update_Correct()
    {
        int id = 1;
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        await context.Products.AddAsync(new Product
        {
            Id = id,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        });
        await context.SaveChangesAsync();
        var productDto = new ProductInputDto
        {
            Name = "Laptop",
            Price = 1234
        };

        var sut = new ProductService(currentTime, context);
        var result = await sut.Update(id, productDto);
        

        Assert.NotNull(result);
        Assert.NotEmpty(context.Products);
        Assert.Equal(productDto.Name, result.Name);
        Assert.Equal(productDto.Price, result.Price);
    }

    [Fact]
    public async Task Update_Incorrect()
    {
        int id = 1;
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        var product = new Product
        {
            Id = id,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        var productDto = new ProductInputDto
        {
            Name = "Laptop",
            Price = 1234
        };

        var sut = new ProductService(currentTime, context);
        var result = Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Update(id + 5, productDto));

        Assert.NotNull(result);
        Assert.NotEmpty(context.Products);
        var originalProduct = await context.Products.FindAsync(id);
        Assert.NotNull(originalProduct);
        Assert.Equal(product.Name, originalProduct.Name);
        Assert.Equal(product.Price, originalProduct.Price);
    }

    [Fact]
    public async Task Delete_Correct()
    {
        int id = 1;
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        await context.Products.AddAsync(new Product
        {
            Id = id,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        });
        await context.SaveChangesAsync();

        var sut = new ProductService(currentTime, context);
        await sut.Delete(id);

        Assert.Empty(context.Products);
    }

    [Fact]
    public async Task Delete_Incorrect()
    {
        int id = 1;
        var context = CreateContext();
        var currentTime = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2024, 2, 27, 12, 0, 0, DateTimeKind.Utc)));
        var product = new Product
        {
            Id = id,
            DateCreated = new DateOnly(2024, 2, 27),
            Name = "Laptop",
            Price = 124
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var sut = new ProductService(currentTime, context);
        var result = Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Delete(id + 5));

        Assert.NotEmpty(context.Products);
        var originalProduct = await context.Products.FindAsync(id);
        Assert.NotNull(originalProduct);
        Assert.Equal(product.Name, originalProduct.Name);
        Assert.Equal(product.Price, originalProduct.Price);
    }

    private static ApplicationDbContext CreateContext()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase("InMemoryForTesting").UseInternalServiceProvider(serviceProvider);

        var context = new ApplicationDbContext(builder.Options);
        return context;
    }
}
