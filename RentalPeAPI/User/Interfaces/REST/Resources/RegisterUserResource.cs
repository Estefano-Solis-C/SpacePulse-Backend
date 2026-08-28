using System.ComponentModel.DataAnnotations;

namespace RentalPeAPI.User.Interfaces.REST.Resources;

public record RegisterUserResource(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(8)]
    string Password,

    [Required]
    string FullName,
    
    string? Phone = null,
    string Role = "customer",
    string? Photo = null

);