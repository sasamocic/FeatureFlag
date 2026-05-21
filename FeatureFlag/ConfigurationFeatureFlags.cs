using Microsoft.Extensions.Configuration;

namespace FeatureFlag;

public sealed class ConfigurationFeatureFlags : IFeatureFlags
{
    public const string DefaultSectionName = "FeatureFlags";

    private readonly IConfiguration _configuration;

    public ConfigurationFeatureFlags(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string SectionName => DefaultSectionName;

    public bool IsEnabled(string featureName)
    {
        if (string.IsNullOrWhiteSpace(featureName))
            return false;

        var value = _configuration[$"{SectionName}:{featureName}"];
        if (string.IsNullOrEmpty(value))
            return false;

        return bool.TryParse(value, out var enabled) && enabled;
    }
}
