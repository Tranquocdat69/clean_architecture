using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECom.Services.Catalog.Infrastructure.EntityConfigurations
{
    public class CatalogEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("catalog_types", CatalogDbContext.DEFAULT_SCHEMA);
            builder.HasKey(t => t.Id);
            builder.Ignore(t => t.DomainEvents);
            builder.Property(t => t.Id);
            builder.Property(t => t.Name)
                .HasColumnName("Catalog_Name");
            builder.HasMany(t => t.CatalogProducts)
                .WithOne()
                .HasForeignKey("CatalogId")
                .OnDelete(DeleteBehavior.Cascade);
            var navigation = builder.Metadata.FindNavigation(nameof(CatalogType.CatalogProducts));

            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
