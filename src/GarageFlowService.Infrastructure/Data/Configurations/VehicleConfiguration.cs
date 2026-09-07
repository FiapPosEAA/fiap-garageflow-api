using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageFlowService.Infrastructure.Data.Configurations;

[ExcludeFromCodeCoverage]
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Brand).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Model).IsRequired().HasMaxLength(100);
        builder.Property(v => v.LicensePlate).IsRequired().HasMaxLength(10);
        builder.Property(v => v.Color).HasMaxLength(50);
        builder.HasIndex(v => v.LicensePlate).IsUnique();
    }
}

