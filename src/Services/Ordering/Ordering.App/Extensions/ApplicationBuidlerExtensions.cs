namespace ECom.Services.Ordering.App.Extensions
{
    public static class ApplicationBuidlerExtensions
    {
        public static IHost UseApplicationConfiguration(this IHost host)
        {
            return host
                .UseDbCobntextMigration<OrderDbContext>((context, services) =>
                {
                    var env = services.GetService<IHostEnvironment>();
                });
        }

        private static IHost UseDbCobntextMigration<TContext>(this IHost host, Action<TContext, IServiceProvider> action) where TContext : DbContext
        {
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger   = services.GetRequiredService<ILogger<TContext>>();
                var context  = services.GetRequiredService<TContext>();
                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                    InvokeSeeder(action, context, services);
                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> action, TContext context, IServiceProvider service) where TContext : DbContext
        {
            if(context != null)
            {
                try
                {
                    context.Database.EnsureCreated();
                }
                finally
                {
                    action(context, service);
                }
            }
        }
    }
}
