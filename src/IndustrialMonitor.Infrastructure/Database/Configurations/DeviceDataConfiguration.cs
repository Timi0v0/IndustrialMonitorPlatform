using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class DeviceDataConfiguration : IEntityTypeConfiguration<DeviceData>
{
    public void Configure(EntityTypeBuilder<DeviceData> builder)
    {
        builder.ToTable("DeviceData");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.CollectTime);
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.RunStatus).HasMaxLength(50);
    }
}
