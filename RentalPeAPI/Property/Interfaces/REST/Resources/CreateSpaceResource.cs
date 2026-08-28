using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Property.Interfaces.Rest.Resources;

/// <summary>
/// DTO para la creación de un espacio (Space/Obra) en el marketplace de remodelaciones.
/// Soporta tanto el contrato REST extendido como la carga útil directa del frontend.
/// </summary>
public class CreateSpaceResource
{
    [JsonPropertyName("homeownerId")]
    public Guid? HomeownerId { get; set; }

    [Required(ErrorMessage = "Title es requerido")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public object? Location { get; set; }

    [JsonPropertyName("type")]
    public object? Type { get; set; }

    [JsonPropertyName("spaceType")]
    public object? SpaceType { get; set; }

    [JsonPropertyName("dimensionsSquareMeters")]
    public decimal? DimensionsSquareMeters { get; set; }

    [JsonPropertyName("estimatedBudget")]
    public decimal? EstimatedBudget { get; set; }

    [JsonPropertyName("pricePerMonth")]
    public decimal? PricePerMonth { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "PEN";

    [JsonPropertyName("hasIot")]
    public bool HasIot { get; set; } = false;

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();
}