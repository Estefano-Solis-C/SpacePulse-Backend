using System;
using System.Collections.Generic;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Interfaces.Rest.Resources;

namespace RentalPeAPI.Property.Interfaces.Rest.Transform;

/// <summary>
/// Ensamblador para convertir DTOs de entrada a Comandos de dominio.
/// Alineado con la nueva estructura de CreateSpaceResource y UpdateSpaceResource.
/// </summary>
public static class SpaceCommandAssembler
{
    public static CreateSpaceCommand ToCommand(CreateSpaceResource resource)
        => new(
            homeownerId: resource.HomeownerId,
            title: resource.Title,
            description: resource.Description,
            location: resource.Location,
            spaceType: resource.SpaceType.ToString(),
            dimensionsSquareMeters: resource.DimensionsSquareMeters,
            estimatedBudget: resource.EstimatedBudget,
            currency: resource.Currency,
            hasIot: resource.HasIot,
            images: resource.Images ?? new List<string>()
        );

    public static UpdateSpaceCommand ToCommand(long id, UpdateSpaceResource resource)
        => new(
            id: id,
            title: resource.Title,
            description: resource.Description,
            location: resource.Location,
            dimensionsSquareMeters: resource.DimensionsSquareMeters,
            estimatedBudget: resource.EstimatedBudget,
            hasIot: resource.HasIot,
            images: resource.Images ?? new List<string>()
        );
}