using LibrarySystem.Domain;
using LibrarySystem.Infrastructure.Data;
using System.Text.Json;

namespace LibrarySystem.Seeding
{
    public static class LibraryContextSeeding
    {
        
        private static readonly string SeedingFolder = Path.Combine(AppContext.BaseDirectory, "Seeding");

        public static async Task SeedAsync(LibraryDbContext dbContext)
        {
            
            if (!dbContext.Authors.Any())
            {
                var authorsData = await File.ReadAllTextAsync(Path.Combine(SeedingFolder, "Authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(authorsData);

                if (authors is not null && authors.Count > 0)
                {
                    await dbContext.Authors.AddRangeAsync(authors);
                    await dbContext.SaveChangesAsync();
                }
            }

           
            if (!dbContext.Borrowers.Any())
            {
                var borrowersData = await File.ReadAllTextAsync(Path.Combine(SeedingFolder, "Borrowers.json"));
                var borrowers = JsonSerializer.Deserialize<List<Borrower>>(borrowersData);

                if (borrowers is not null && borrowers.Count > 0)
                {
                    await dbContext.Borrowers.AddRangeAsync(borrowers);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}