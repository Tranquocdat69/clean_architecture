using Microsoft.EntityFrameworkCore;

namespace ECom.Services.Balance.App.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private const string DB_CONNECTION_KEY = "BalanceDBSqlServer";

        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder, IConfiguration configuration) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<TContext>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    InvokeSeeder(configuration, seeder, context, services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    throw;
                }
            }
            return host;
        }

        public static void InvokeSeeder<TContext>(IConfiguration configuration, Action<TContext, IServiceProvider> seeder, TContext? context, IServiceProvider serviceProvider) where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(DB_CONNECTION_KEY);

            if (context != null)
            {
                if (!String.IsNullOrEmpty(connectionString))
                {
                    context.Database.Migrate();
                }
                seeder(context, serviceProvider);
            }
        }

        /*public static IHost InitBalanceRingBuffer(this IHost host, IConfiguration configuration)
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<LogHandler>>();

            var inputRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("InputRingSize").Value);
            var persistentRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("PersistentRingSize").Value);
            var replyRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("ReplyRingSize").Value);
            var numberOfLogHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfLogHandlers").Value);
            var numberOfReplyHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfReplyHandlers").Value);

            var inputDisruptor = new Disruptor<UpdateCreditLimitEvent>(() => new UpdateCreditLimitEvent(), inputRingSize);
            var persistentDisruptor = new Disruptor<UpdateCreditLimitPersistentEvent>(() => new UpdateCreditLimitPersistentEvent(), persistentRingSize);
            var replyDisruptor = new Disruptor<UpdateCreditLimitReplyEvent>(() => new UpdateCreditLimitReplyEvent(), replyRingSize);

            inputDisruptor.HandleEventsWith(GetLogHandlers(logger, numberOfLogHandlers))
            .Then(new BusinessHandler());
            replyDisruptor.HandleEventsWith(GetReplyHandlers(numberOfReplyHandlers));

            inputDisruptor.Start();
            persistentDisruptor.Start();
            replyDisruptor.Start();

            return host;
        }

        private static LogHandler[] GetLogHandlers(ILogger<LogHandler> logger, int size)
        {
            LogHandler[] logHandlers = new LogHandler[size];

            for (int i = 0; i < size; i++)
            {
                logHandlers[i] = new LogHandler(logger, i + 1);
            }

            return logHandlers;
        }
        private static IntegrationReplyHandler[] GetReplyHandlers(int size)
        {
            IntegrationReplyHandler[] replyHandlers = new IntegrationReplyHandler[size];

            for (int i = 0; i < size; i++)
            {
                replyHandlers[i] = new IntegrationReplyHandler(i + 1);
            }

            return replyHandlers;
        }*/
    }
}
