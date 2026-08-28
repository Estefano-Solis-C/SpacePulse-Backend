using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para crear un dispositivo IoT vinculado a un espacio con autocompletado de métricas.
/// Tipos permitidos: HUMIDITY, TEMPERATURE, VOLTAGE, LOAD, AIR_QUALITY, OTHER, OTHERS.
/// </summary>
public record CreateIoTDeviceResource(
    [Required(ErrorMessage = "SpaceId es requerido")]
    [property: JsonPropertyName("spaceId")]
    long SpaceId,

    [Required(ErrorMessage = "Type es requerido")]
    [property: JsonPropertyName("type")]
    string Type,

    [Required(ErrorMessage = "Name es requerido")]
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("serialNumber")]
    string? SerialNumber,

    [property: JsonPropertyName("customMetricName")]
    string? CustomMetricName,

    [property: JsonPropertyName("customUnit")]
    string? CustomUnit,

    [property: JsonPropertyName("customMinThreshold")]
    decimal? CustomMinThreshold,

    [property: JsonPropertyName("customMaxThreshold")]
    decimal? CustomMaxThreshold
);