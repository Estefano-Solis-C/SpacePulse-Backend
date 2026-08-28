using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para presentar los datos completos de una tarea (WorkItem).
/// Mapea todas las propiedades de la entidad de dominio.
/// </summary>
public record WorkItemResource(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("createdByUserId")] Guid CreatedByUserId,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("photoUrl")] string? PhotoUrl,
    [property: JsonPropertyName("plannedStartDate")] DateTime? PlannedStartDate,
    [property: JsonPropertyName("plannedEndDate")] DateTime? PlannedEndDate,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("completedAt")] DateTime? CompletedAt
);