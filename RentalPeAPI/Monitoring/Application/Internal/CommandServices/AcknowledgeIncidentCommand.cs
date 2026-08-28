
using MediatR;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public record AcknowledgeIncidentCommand(
    int IncidentId,
    Guid AcknowledgedByUserId 
) : IRequest<bool>; 