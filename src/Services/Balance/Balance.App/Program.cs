using ECom.Services.Balance.App.Extensions;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.UseServiceCollectionConfiguration(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MigrateDbContext<UserDbContext>((context, sp) =>
{
    var userRepository = sp.GetRequiredService<IUserRepository>();
    var env = sp.GetRequiredService<IHostEnvironment>();
    int numberOfLogHanlers = Int32.Parse(builder.Configuration.GetSection("Disruptor").GetSection("NumberOfLogHandlers").Value);

    new UserDbContextSeed().SeedAsync(context, userRepository, env, numberOfLogHanlers).Wait();
    new KafkaOffsetSeed().SeedAsync(env).Wait();
}, builder.Configuration);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
