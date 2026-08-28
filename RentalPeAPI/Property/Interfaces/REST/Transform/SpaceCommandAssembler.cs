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
    public static CreateSpaceCommand ToCommand(CreateSpaceResource resource, Guid? overrideHomeownerId = null)
    {
        Guid homeownerId = overrideHomeownerId ?? resource.HomeownerId ?? Guid.Empty;

        string locationStr = "Lima, Peru";
        if (resource.Location is System.Text.Json.JsonElement locElem)
        {
            if (locElem.ValueKind == System.Text.Json.JsonValueKind.String)
                locationStr = locElem.GetString() ?? "Lima, Peru";
            else if (locElem.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                var addr = locElem.TryGetProperty("address", out var a) ? a.GetString() : "";
                var city = locElem.TryGetProperty("city", out var c) ? c.GetString() : "Lima";
                var country = locElem.TryGetProperty("country", out var co) ? co.GetString() : "Peru";
                locationStr = $"{addr}, {city}, {country}".Trim(',', ' ');
            }
        }
        else if (resource.Location != null)
        {
            locationStr = resource.Location.ToString() ?? "Lima, Peru";
        }

        string spaceTypeStr = resource.Type?.ToString() ?? resource.SpaceType?.ToString() ?? "Apartment";
        decimal budget = resource.EstimatedBudget ?? resource.PricePerMonth ?? 1200m;
        decimal dimensions = resource.DimensionsSquareMeters ?? 65.0m;

        return new CreateSpaceCommand(
            homeownerId: homeownerId,
            title: resource.Title,
            description: string.IsNullOrWhiteSpace(resource.Description) ? "Modern space" : resource.Description,
            location: locationStr,
            spaceType: spaceTypeStr,
            dimensionsSquareMeters: dimensions,
            estimatedBudget: budget,
            currency: resource.Currency ?? "PEN",
            hasIot: resource.HasIot,
            images: resource.Images ?? new List<string>()
        );
    }

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