namespace ECom.Services.Balance.Infrastructure
#nullable disable
{
    public class UserDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "Balance";

        public DbSet<User> Users { get; set; }
        public DbSet<KafkaOffset> KafkaOffsets { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new KafkaOffsetEntityTypeConfiguration());
        }
    }
}
