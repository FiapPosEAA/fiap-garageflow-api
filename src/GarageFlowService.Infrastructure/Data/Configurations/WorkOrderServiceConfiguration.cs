using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageFlowService.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class WorkOrderServiceConfiguration : IEntityTypeConfiguration<WorkOrderService>
{
    public void Configure(EntityTypeBuilder<WorkOrderService> builder)
    {
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.UnitPrice).HasPrecision(18, 2);
        builder.Ignore(ws => ws.Subtotal);

        builder.HasOne(ws => ws.Service)
            .WithMany()
            .HasForeignKey(ws => ws.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

