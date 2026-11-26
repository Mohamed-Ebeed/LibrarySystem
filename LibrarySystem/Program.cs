using LibrarySystem.Extensions;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Seeding;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // كل التسجيلات في ApplicationServiceExtension
            builder.Services.AddServices(builder.Configuration);

            var app = builder.Build();

            #region Auto Migration + Seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = services.GetRequiredService<LibraryDbContext>();
                    await dbContext.Database.MigrateAsync();
                    await LibraryContextSeeding.SeedAsync(dbContext);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while migrating / seeding the database");
                }
            }
            #endregion

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}