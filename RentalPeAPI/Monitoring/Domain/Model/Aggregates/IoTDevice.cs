using System;

namespace RentalPeAPI.Monitoring.Domain.Model.Aggregates;

/// <summary>
/// Dispositivo IoT vinculado a un espacio (Space) con simulación automática de telemetría
/// y validaciones de seguridad por usuario creador.
/// </summary>
public class IoTDevice
{
    // Tipos de sensores permitidos
    private static readonly HashSet<string> AllowedTypes = new()
    {
        "HUMIDITY", "TEMPERATURE", "VOLTAGE", "LOAD", "AIR_QUALITY", "OTHER", "OTHERS"
    };

    // Mapa de autocompletado de métricas
    private static readonly Dictionary<string, (string MetricName, string Unit)> MetricMap = new()
    {
        { "HUMIDITY", ("Humedad Relativa", "%") },
        { "TEMPERATURE", ("Temperatura", "C°") },
        { "VOLTAGE", ("Voltaje", "V") },
        { "LOAD", ("Carga", "kg") },
        { "AIR_QUALITY", ("Calidad del Aire", "AQI") }
    };

    // Propiedades de solo lectura con encapsulamiento
    public long Id { get; private set; }
    public long SpaceId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public string MetricName { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public DateTime Timestamp { get; private set; }
    public bool IsOn { get; private set; }
    
    // Propiedades privadas para umbrales y estado de alerta
    private decimal MinThreshold { get; set; }
    private decimal MaxThreshold { get; set; }
    private bool IsInAlertState { get; set; }

    // Constructor privado para EF Core
    private IoTDevice() { }

    /// <summary>
    /// Constructor principal para crear un nuevo dispositivo IoT con autocompletado de métricas.
    /// </summary>
    /// <param name="spaceId">ID del espacio propietario del dispositivo</param>
    /// <param name="createdByUserId">ID del usuario que crea el dispositivo</param>
    /// <param name="type">Tipo de sensor (HUMIDITY, TEMPERATURE, VOLTAGE, LOAD, AIR_QUALITY, OTHER, OTHERS)</param>
    /// <param name="name">Nombre del dispositivo</param>
    /// <param name="serialNumber">Número de serie del dispositivo</param>
    /// <param name="customMetricName">Nombre de métrica personalizada (requerida si type es OTHER/OTHERS)</param>
    /// <param name="customUnit">Unidad personalizada (requerida si type es OTHER/OTHERS)</param>
    /// <param name="customMinThreshold">Umbral mínimo personalizado (requerido si type es OTHER/OTHERS)</param>
    /// <param name="customMaxThreshold">Umbral máximo personalizado (requerido si type es OTHER/OTHERS)</param>
    public IoTDevice(
        long spaceId,
        Guid createdByUserId,
        string type,
        string name,
        string serialNumber,
        string? customMetricName = null,
        string? customUnit = null,
        decimal? customMinThreshold = null,
        decimal? customMaxThreshold = null)
    {
        if (spaceId <= 0)
            throw new ArgumentException("SpaceId debe ser > 0", nameof(spaceId));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId debe ser un GUID válido", nameof(createdByUserId));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type es obligatorio", nameof(type));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name es obligatorio", nameof(name));

        // Convertir a mayúsculas y validar tipo
        Type = type.ToUpperInvariant();
        if (!AllowedTypes.Contains(Type))
            throw new ArgumentException(
                $"Type '{Type}' no permitido. Opciones: {string.Join(", ", AllowedTypes)}",
                nameof(type));

        // Autocompletar MetricName y Unit según el tipo
        if (MetricMap.TryGetValue(Type, out var metrics))
        {
            MetricName = metrics.MetricName;
            Unit = metrics.Unit;
        }
         else if (Type is "OTHER" or "OTHERS")
         {
             // Para tipos personalizados, validar que se proporcionen los valores
             if (string.IsNullOrWhiteSpace(customMetricName))
                 throw new ArgumentException(
                     "customMetricName es obligatorio cuando Type es OTHER u OTHERS",
                     nameof(customMetricName));

             if (string.IsNullOrWhiteSpace(customUnit))
                 throw new ArgumentException(
                     "customUnit es obligatorio cuando Type es OTHER u OTHERS",
                     nameof(customUnit));

             MetricName = customMetricName;
             Unit = customUnit;
         }
         
         // Inicializar umbrales según el tipo de dispositivo
         (MinThreshold, MaxThreshold) = Type switch
         {
             "HUMIDITY" => (30, 70),
             "TEMPERATURE" => (15, 30),
             "VOLTAGE" => (200, 240),
             "LOAD" => (0, 350),
             "AIR_QUALITY" => (0, 100),
             "OTHER" or "OTHERS" when customMinThreshold.HasValue && customMaxThreshold.HasValue 
                 => (customMinThreshold.Value, customMaxThreshold.Value),
             "OTHER" or "OTHERS" 
                 => throw new ArgumentException(
                     "customMinThreshold y customMaxThreshold son obligatorios para tipos personalizados (OTHER/OTHERS).",
                     nameof(type)),
             _ => throw new ArgumentException(
                 "Tipo de dispositivo no reconocido.",
                 nameof(type))
         };

        SpaceId = spaceId;
        CreatedByUserId = createdByUserId;
        Name = name;
        SerialNumber = serialNumber ?? string.Empty;
        IsOn = true;
        IsInAlertState = false;
        Timestamp = DateTime.UtcNow;
        GenerateRandomValue(); // Genera el valor inicial con lógica inteligente
    }

    /// <summary>
    /// Actualiza el nombre y número de serie del dispositivo.
    /// </summary>
    public void UpdateDetails(string name, string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name es obligatorio", nameof(name));

        Name = name;
        SerialNumber = serialNumber ?? string.Empty;
    }

    /// <summary>
    /// Alterna el estado de encendido/apagado del dispositivo.
    /// Si se enciende, genera inmediatamente un nuevo valor de telemetría.
    /// </summary>
    public void TogglePower()
    {
        IsOn = !IsOn;

        // Si se enciende, generar un nuevo valor inmediatamente
        if (IsOn)
        {
            GenerateRandomValue();
        }
    }

    /// <summary>
    /// Apaga el dispositivo sin generar telemetría.
    /// Método de dominio simple para desactivar dispositivos de forma automática.
    /// </summary>
    public void TurnOff()
    {
        IsOn = false;
    }

    /// <summary>
    /// Genera un valor de telemetría inteligente según el tipo de dispositivo.
    /// Implementa lógica realista con 85% de valores normales y 15% de picos extremos
    /// para simular comportamientos anómalos y probar alertas.
    /// Todos los valores se generan como decimales con 2 cifras para mayor realismo.
    /// Solo se ejecuta si el dispositivo está encendido (IsOn = true).
    /// </summary>
    public void GenerateRandomValue()
    {
        if (!IsOn)
            return;

        // Lógica de simulación oscilante realista:
        // 85% genera valor normal dentro del rango estable
        // 15% genera valor extremo para forzar cruce de umbrales y probar alertas
        bool triggerAnomaly = Random.Shared.NextDouble() > 0.85;

        // Generar valores decimales con 2 cifras usando NextDouble()
        Value = Type switch
        {
            "HUMIDITY" => triggerAnomaly 
                ? (decimal)Math.Round(20 + Random.Shared.NextDouble() * 65, 2)
                : (decimal)Math.Round(40 + Random.Shared.NextDouble() * 25, 2),
            "TEMPERATURE" => triggerAnomaly 
                ? (decimal)Math.Round(10 + Random.Shared.NextDouble() * 30, 2)
                : (decimal)Math.Round(18 + Random.Shared.NextDouble() * 8, 2),
            "VOLTAGE" => triggerAnomaly 
                ? (decimal)Math.Round(180 + Random.Shared.NextDouble() * 80, 2)
                : (decimal)Math.Round(215 + Random.Shared.NextDouble() * 15, 2),
            "LOAD" => triggerAnomaly 
                ? (decimal)Math.Round(300 + Random.Shared.NextDouble() * 150, 2)
                : (decimal)Math.Round(50 + Random.Shared.NextDouble() * 200, 2),
            "AIR_QUALITY" => triggerAnomaly 
                ? (decimal)Math.Round(90 + Random.Shared.NextDouble() * 60, 2)
                : (decimal)Math.Round(15 + Random.Shared.NextDouble() * 35, 2),
            _ => (decimal)Math.Round((double)MinThreshold - 10 + Random.Shared.NextDouble() * ((double)(MaxThreshold - MinThreshold) + 20), 2)
        };

        Timestamp = DateTime.UtcNow;
        
        // Evaluar si se ha cruzado umbral
        EvaluateThresholds();
    }
    
    /// <summary>
    /// Evalúa si el valor actual ha cruzado los umbrales mínimo o máximo.
    /// Este método es preparatorio para futuros sistemas de alertas.
    /// </summary>
    private void EvaluateThresholds()
    {
        IsInAlertState = Value < MinThreshold || Value > MaxThreshold;
    }

    /// <summary>
    /// Obtiene el estado actual de alerta del dispositivo.
    /// Utilizado internamente para evaluar si se debe disparar una notificación.
    /// </summary>
    public bool GetIsInAlertState()
    {
        return IsInAlertState;
    }

    /// <summary>
    /// Actualiza explícitamente el estado de alerta del dispositivo.
    /// Utilizado para marcar el dispositivo en alerta tras detectar anomalía.
    /// </summary>
    public void UpdateAlertState(bool isInAlert)
    {
        IsInAlertState = isInAlert;
    }

    /// <summary>
    /// Expone los umbrales para lógica de validación externa.
    /// </summary>
    public (decimal MinThreshold, decimal MaxThreshold) GetThresholds()
    {
        return (MinThreshold, MaxThreshold);
    }
}