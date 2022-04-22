namespace ECom.Services.Ordering.Infrastructure.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.ToTable("orders", OrderDbContext.DEFAULT_SCHEMA);

        orderConfiguration.HasKey(o => o.Id);

        orderConfiguration.Ignore(b => b.DomainEvents);

        orderConfiguration.Property(o => o.Id);

        //Address value object persisted as owned entity type supported since EF Core 2.0
        orderConfiguration
            .OwnsOne(o => o.Address, a =>
            {
                // Explicit configuration of the shadow key property in the owned type 
                // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740
                a.Property<int>("OrderId");
                a.WithOwner();
            });

        orderConfiguration.Property(o => o.OrderDate)
            .HasColumnName("Order_Date");
        orderConfiguration.Property(o => o.CustomerId)
            .HasColumnName("User_Id");

        var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));
    }
}
