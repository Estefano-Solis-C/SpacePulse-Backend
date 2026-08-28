using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la presentación REST de una notificación.
/// Expone únicamente los campos permitidos para el cliente frontend.
/// </summary>
public record NotificationResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("isRead")] bool IsRead,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt
);

