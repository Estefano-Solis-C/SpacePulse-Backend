using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para presentar los datos de un dispositivo IoT.
/// </summary>
public record IoTDeviceResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("installedAt")] DateTime InstalledAt
);