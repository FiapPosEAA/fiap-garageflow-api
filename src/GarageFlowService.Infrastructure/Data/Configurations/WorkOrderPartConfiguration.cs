using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageFlowService.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class WorkOrderPartConfiguration : IEntityTypeConfiguration<WorkOrderPart>
{
    public void Configure(EntityTypeBuilder<WorkOrderPart> builder)
    {
        builder.HasKey(wp => wp.Id);
        builder.Property(wp => wp.UnitPrice).HasPrecision(18, 2);
        builder.Ignore(wp => wp.Subtotal);

        builder.HasOne(wp => wp.Part)
            .WithMany()
            .HasForeignKey(wp => wp.PartId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

