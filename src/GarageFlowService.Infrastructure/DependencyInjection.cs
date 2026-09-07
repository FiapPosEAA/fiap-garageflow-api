using System.Diagnostics.CodeAnalysis;
using GarageFlowService.Application.Interfaces;
using GarageFlowService.Domain.Interfaces;
using GarageFlowService.Infrastructure.Data;
using GarageFlowService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GarageFlowService.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, global::GarageFlowService.Infrastructure.UnitOfWork.UnitOfWork>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();

        return services;
    }
}

