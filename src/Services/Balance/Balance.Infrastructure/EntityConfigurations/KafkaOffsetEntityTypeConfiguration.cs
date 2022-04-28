namespace ECom.Services.Balance.Infrastructure.EntityConfigurations;

class KafkaOffsetEntityTypeConfiguration : IEntityTypeConfiguration<KafkaOffset>
{
    public void Configure(EntityTypeBuilder<KafkaOffset> userConfiguration)
    {
        userConfiguration.ToTable("kafkaoffset");

        userConfiguration.HasKey(k => k.Id);

        userConfiguration.Property(k => k.Id);

        userConfiguration.Property(k => k.CommandOffset)
            .HasColumnName("Command_Offset")
            .HasColumnType("bigint");

        userConfiguration.Property(k => k.PersistentOffset)
            .HasColumnName("Persistent_Offset")
            .HasColumnType("bigint");
    }
}
