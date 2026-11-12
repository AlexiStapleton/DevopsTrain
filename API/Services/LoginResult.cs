namespace API.Services;

public enum LoginStatus
{
    Success,
    InvalidCredentials   // meme resultat pour "email inconnu" et "mauvais mot de passe"
}
 
public record LoginResult(LoginStatus Status, string? Token = null);