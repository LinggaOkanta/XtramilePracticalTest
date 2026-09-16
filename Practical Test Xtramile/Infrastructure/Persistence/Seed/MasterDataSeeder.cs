using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Infrastructure.Persistence.Seed;

public static class MasterDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Guard check: Avoid duplicating seed data
        if (await context.Countries.AnyAsync())
        {
            return;
        }

        var countries = new List<Country>
        {
            new Country(1, "AU", "Australia"),
            new Country(2, "US", "United States"),
            new Country(3, "ID", "Indonesia"),
            new Country(4, "GB", "United Kingdom"),
            new Country(5, "JP", "Japan")
        };

        var cities = new List<City>
        {
            // Australia
            new City(101, 1, "Melbourne"),
            new City(102, 1, "Sydney"),
            new City(103, 1, "Brisbane"),
            new City(104, 1, "Perth"),

            // United States
            new City(201, 2, "New York"),
            new City(202, 2, "San Francisco"),
            new City(203, 2, "Chicago"),
            new City(204, 2, "Seattle"),

            // Indonesia
            new City(301, 3, "Jakarta"),
            new City(302, 3, "Surabaya"),
            new City(303, 3, "Bandung"),
            new City(304, 3, "Bali"),

            // United Kingdom
            new City(401, 4, "London"),
            new City(402, 4, "Manchester"),
            new City(403, 4, "Edinburgh"),

            // Japan
            new City(501, 5, "Tokyo"),
            new City(502, 5, "Osaka"),
            new City(503, 5, "Kyoto")
        };

        await context.Countries.AddRangeAsync(countries);
        await context.Cities.AddRangeAsync(cities);
        await context.SaveChangesAsync();
    }
}
