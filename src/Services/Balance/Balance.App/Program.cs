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
    var userDbContext = sp.GetRequiredService<UserDbContext>();
    var userRepository = sp.GetRequiredService<IUserRepository>();
    var env = sp.GetRequiredService<IHostEnvironment>();

    int numberOfLogHanlers = Int32.Parse(builder.Configuration.GetSection("Disruptor").GetSection("NumberOfLogHandlers").Value);
    int numberOfReplyHandlers = Int32.Parse(builder.Configuration.GetSection("Disruptor").GetSection("NumberOfReplyHandlers").Value);

    new UserDbContextSeed().SeedAsync(userDbContext, userRepository, env, numberOfLogHanlers, numberOfReplyHandlers).Wait();
}, builder.Configuration);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
