using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

/// <summary>
/// Comando para crear un dispositivo IoT vinculado a un espacio (Space).
/// Incluye validaciones de seguridad por usuario creador y autocompletado de métricas.
/// </summary>
public record CreateIoTDeviceCommand(
    long SpaceId,
    Guid CreatedByUserId,
    string Type,
    string Name,
    string? SerialNumber,
    string? CustomMetricName,
    string? CustomUnit,
    decimal? CustomMinThreshold,
    decimal? CustomMaxThreshold
) : IRequest<IoTDevice>;