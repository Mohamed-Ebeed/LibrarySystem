using LibrarySystem.Helper;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Infrastructure.Interceptors;
using LibrarySystem.Repositories;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Filters;
using LibrarySystem.Middleware;

namespace LibrarySystem.Extensions
{
    public static class ApplicationServiceExtension
    {
        public const string CorsPolicyName = "ConfigurationOrigins";
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Session 01 - EF Core + Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{typeof(ApplicationServiceExtension).Assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultSqlConnection"));
                // Session 03 - Interceptors
                options.AddInterceptors(new SoftDeleteInterceptor(), new CreatedAtInterceptor(), new ModifiedAtInterceptor());
            });
            #endregion

            #region Session 02 - AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AuthorProfile>();
                cfg.AddProfile<BorrowerProfile>();
                cfg.AddProfile<LoanProfile>();
            });
            #endregion

            #region Session 03 - Repository + Service
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBorrowerService, BorrowerService>();
            services.AddScoped<ILoanService, LoanService>();
            #endregion
            #region Session 04 - Middlewares, Filters, CORS
            services.AddTransient<RequestIdHeaderMiddleware>();

            services.AddScoped<ActionExecutionLoggingFilter>();
            services.AddScoped<ActionTimingFilter>();
            services.AddScoped<ExceptionHandlingFilter>();

            var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options =>
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    // بس الـ origins اللي في appsettings تقدر تنادي الـ API من متصفح
                    if (corsOrigins.Length > 0)
                        policy.WithOrigins(corsOrigins);

                    policy.AllowAnyHeader().AllowAnyMethod();
                }));
            #endregion
            return services;
        }
    }
}