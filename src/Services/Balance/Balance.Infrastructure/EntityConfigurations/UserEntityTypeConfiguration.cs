namespace ECom.Services.Balance.Infrastructure.EntityConfigurations;

class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> userConfiguration)
    {
        userConfiguration.ToTable("userbalance");

        userConfiguration.HasKey(u => u.Id);

        userConfiguration.Ignore(b => b.DomainEvents);

        userConfiguration.Property(u => u.Id);

        userConfiguration.Property(u => u.Name)
            .HasColumnName("User_Name");

        userConfiguration.Property(u => u.CreditLimit)
            .HasColumnName("Credit_Limit")
            .HasColumnType("decimal(18,0)");
    }
}
