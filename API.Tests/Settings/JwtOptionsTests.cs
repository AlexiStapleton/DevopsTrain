using System.ComponentModel.DataAnnotations;
using API.Settings;

namespace API.Tests.Settings;

public class JwtOptionsTests
{
    private const string ValidKey = "0123456789-0123456789-0123456789-01"; // 35 caracteres

    private static bool IsValid(JwtOptions options) =>
        Validator.TryValidateObject(
            options,
            new ValidationContext(options),
            new List<ValidationResult>(),
            validateAllProperties: true);

    [Fact]
    public void Options_WithAllValues_AreValid()
    {
        var options = new JwtOptions { Key = ValidKey, Issuer = "issuer", Audience = "audience" };

        Assert.True(IsValid(options));
    }

    [Theory]
    [InlineData("short", "issuer", "audience")]   // cle trop courte
    [InlineData("", "issuer", "audience")]        // cle absente
    [InlineData(ValidKey, "", "audience")]        // issuer absent
    [InlineData(ValidKey, "issuer", "")]          // audience absente
    public void Options_WithMissingOrWeakValues_AreInvalid(string key, string issuer, string audience)
    {
        var options = new JwtOptions { Key = key, Issuer = issuer, Audience = audience };

        Assert.False(IsValid(options));
    }
}