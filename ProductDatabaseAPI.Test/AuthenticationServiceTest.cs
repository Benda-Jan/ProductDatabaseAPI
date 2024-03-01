using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductDatabaseAPI.Data;
using ProductDatabaseAPI.Models;
using ProductDatabaseAPI.Dtos;
using ProductDatabaseAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestPlatform.Common;
using Microsoft.AspNetCore.Identity.Test;

namespace ProductDatabaseAPI.Test;

public class AuthenticationServiceTest
{
    [Fact]
    public async Task Login_Correct()
    {
        throw new Exception();
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
