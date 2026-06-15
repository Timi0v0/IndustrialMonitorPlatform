using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceCode).IsUnique();
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.DeviceName).HasMaxLength(100);
        builder.Property(x => x.DeviceType).HasMaxLength(50);
        builder.Property(x => x.ProtocolType).HasMaxLength(50);
        builder.Property(x => x.IpAddress).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
    }
}
