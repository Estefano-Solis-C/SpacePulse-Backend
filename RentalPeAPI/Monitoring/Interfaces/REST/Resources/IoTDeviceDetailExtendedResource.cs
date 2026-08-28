using System;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO extendido que presenta el detalle completo de un dispositivo IoT con telemetría,
/// umbrales e información de estado de alerta. Incluye metadata para monitoreo avanzado.
/// Los valores se retornan con máximo 2 decimales para presentación clara.
/// </summary>
public record IoTDeviceDetailExtendedResource(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("spaceId")] long SpaceId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("serialNumber")] string SerialNumber,
    [property: JsonPropertyName("metricName")] string MetricName,
    [property: JsonPropertyName("unit")] string Unit,
    [property: JsonPropertyName("value")] decimal Value,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp,
    [property: JsonPropertyName("isOn")] bool IsOn,
    [property: JsonPropertyName("minThreshold")] decimal MinThreshold,
    [property: JsonPropertyName("maxThreshold")] decimal MaxThreshold,
    [property: JsonPropertyName("isInAlertState")] bool IsInAlertState
);

