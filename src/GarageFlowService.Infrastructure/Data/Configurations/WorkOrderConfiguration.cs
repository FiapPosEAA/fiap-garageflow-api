using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageFlowService.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.OrderNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(w => w.OrderNumber).IsUnique();
        builder.Property(w => w.Description).IsRequired().HasMaxLength(1000);
        builder.Property(w => w.DiagnosisNotes).HasMaxLength(2000);
        builder.Property(w => w.TotalAmount).HasPrecision(18, 2);
        builder.Property(w => w.Status).IsRequired();

        builder.HasOne(w => w.Customer)
            .WithMany()
            .HasForeignKey(w => w.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Vehicle)
            .WithMany()
            .HasForeignKey(w => w.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.WorkOrderServices)
            .WithOne()
            .HasForeignKey(ws => ws.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.WorkOrderParts)
            .WithOne()
            .HasForeignKey(wp => wp.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

