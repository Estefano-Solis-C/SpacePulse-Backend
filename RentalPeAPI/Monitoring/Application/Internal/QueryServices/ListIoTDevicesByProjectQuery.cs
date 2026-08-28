using System.Collections.Generic;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

/// <summary>
/// Query para listar dispositivos IoT asociados a un espacio específico.
/// </summary>
public record ListIoTDevicesBySpaceQuery(long SpaceId)
    : IRequest<IEnumerable<IoTDevice>>;