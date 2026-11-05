using System.ComponentModel.DataAnnotations;
namespace API.Settings;

/// <summary>
/// Configuration JWT, liee a la section "Jwt" de la configuration
/// (variables d'environnement Jwt__Key, Jwt__Issuer, Jwt__Audience).
/// </summary>
public class JwtOptions
{
    // HS256 exige une cle d'au moins 256 bits (32 caracteres)
    [Required, MinLength(32)]
    public string Key { get; set; } = string.Empty;
 
    [Required]
    public string Issuer { get; set; } = string.Empty;
 
    [Required]
    public string Audience { get; set; } = string.Empty;
}