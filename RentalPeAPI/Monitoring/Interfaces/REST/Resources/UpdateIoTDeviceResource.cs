using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para actualizar solo nombre y número de serie de un dispositivo IoT.
/// </summary>
public record UpdateIoTDeviceResource(
    [Required(ErrorMessage = "Name es requerido")]
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("serialNumber")]
    string? SerialNumber
);
