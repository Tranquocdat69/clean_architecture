using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Services.Catalog.Infrastructure.EntityConfigurations
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product", CatalogDbContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);
            builder.Ignore(x => x.DomainEvents);
            builder.Property<int>("CatalogId").IsRequired();
            builder.Property(x => x.Name).HasColumnName("ProductName");
            builder.Property(x => x.AvailableStock).HasColumnName("Available_Stock");
            builder.Property(x => x.Price).HasColumnName("ProductPrice");
        }
    }
}
