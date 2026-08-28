using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO resumido para dispositivos IoT que expone solo información básica de seguridad.
/// Excluye propiedades sensibles como CreatedByUserId.
/// </summary>
public record IoTDeviceSummaryResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("serialNumber")] string SerialNumber
);

