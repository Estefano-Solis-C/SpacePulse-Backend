using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Property.Interfaces.Rest.Resources;

/// <summary>
/// DTO para actualizar un espacio (Space).
/// Alineado estrictamente con el contrato JSON definitivo.
/// </summary>
public class UpdateSpaceResource
{
    [StringLength(100, ErrorMessage = "Title no puede exceder 100 caracteres")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description no puede exceder 500 caracteres")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Location no puede exceder 200 caracteres")]
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [Range(0.01, float.MaxValue, ErrorMessage = "DimensionsSquareMeters debe ser mayor a 0")]
    [JsonPropertyName("dimensionsSquareMeters")]
    public decimal DimensionsSquareMeters { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "EstimatedBudget debe ser mayor a 0")]
    [JsonPropertyName("estimatedBudget")]
    public decimal EstimatedBudget { get; set; }

    [JsonPropertyName("hasIot")]
    public bool HasIot { get; set; } = false;

    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();
}