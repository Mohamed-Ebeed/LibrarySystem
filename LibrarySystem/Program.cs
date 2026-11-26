using LibrarySystem.Helper;
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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection")));

            #region Session 02 - AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AuthorProfile>();
                cfg.AddProfile<BorrowerProfile>();
                cfg.AddProfile<LoanProfile>();
            });
            #endregion

            var app = builder.Build();

            #region Session 02 - Auto Migration + Seeding
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