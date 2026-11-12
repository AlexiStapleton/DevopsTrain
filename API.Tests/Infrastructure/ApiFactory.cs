using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace API.Tests.Infrastructure;

/// <summary>
/// Demarre l'API reelle en remplacant la configuration par celle des tests :
/// base PostgreSQL du conteneur + valeurs JWT de test.
/// </summary>
public class ApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    // Valeurs de test uniquement (jamais utilisees hors des tests)
    public const string JwtKey = "test-only-signing-key-0123456789-abcdefghijklmnop";
    public const string JwtIssuer = "test-issuer";
    public const string JwtAudience = "test-audience";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Ajoutee apres les sources par defaut : elle l'emporte sur l'environnement et les user-secrets
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString,
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience
            });
        });
    }
}