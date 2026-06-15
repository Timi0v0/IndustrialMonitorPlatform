using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class AlarmRecordConfiguration : IEntityTypeConfiguration<AlarmRecord>
{
    public void Configure(EntityTypeBuilder<AlarmRecord> builder)
    {
        builder.ToTable("AlarmRecords");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.AlarmTime);
        builder.Property(x => x.DeviceCode).HasMaxLength(50);
        builder.Property(x => x.AlarmType).HasMaxLength(50);
        builder.Property(x => x.AlarmLevel).HasMaxLength(50);
        builder.Property(x => x.AlarmMessage).HasMaxLength(500);
        builder.Property(x => x.ConfirmUser).HasMaxLength(50);
    }
}
