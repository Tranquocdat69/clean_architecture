using ECom.BuildingBlocks.SharedKernel.Extensions;

namespace ECom.Services.Balance.Infrastructure
#nullable disable
{
    public class UserDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "Balance";

        public DbSet<User> Users { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
