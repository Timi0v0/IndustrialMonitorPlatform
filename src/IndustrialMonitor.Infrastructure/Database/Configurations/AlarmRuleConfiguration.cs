using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class AlarmRuleConfiguration : IEntityTypeConfiguration<AlarmRule>
{
    public void Configure(EntityTypeBuilder<AlarmRule> builder)
    {
        builder.ToTable("AlarmRules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId);
        builder.Property(x => x.DataKey).HasMaxLength(50);
        builder.Property(x => x.Operator).HasMaxLength(10);
        builder.Property(x => x.AlarmLevel).HasMaxLength(50);
        builder.Property(x => x.AlarmMessage).HasMaxLength(500);
    }
}
