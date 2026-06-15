using IndustrialMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndustrialMonitor.Infrastructure.Database.Configurations;

public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.ToTable("OperationLogs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.OperationTime);
        builder.Property(x => x.UserName).HasMaxLength(50);
        builder.Property(x => x.OperationType).HasMaxLength(50);
        builder.Property(x => x.Result).HasMaxLength(50);
    }
}
