using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FeatureFlag.Tests;

internal static class ConfigurationTestHelpers
{
    public static IConfiguration FromJson(string json)
    {
        return new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
            .Build();
    }

    /// <summary>
    /// Builds configuration from flat keys such as <c>FeatureFlags:MyFlag</c> without referencing the Memory provider package.
    /// </summary>
    public static IConfiguration FromKeyValue(params KeyValuePair<string, string?>[] pairs)
    {
        var section = new Dictionary<string, string?>(StringComparer.Ordinal);
        var prefix = ConfigurationFeatureFlags.SectionName + ":";

        foreach (var pair in pairs)
        {
            if (!pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                throw new ArgumentException($"Key must start with '{prefix}'.", nameof(pairs));

            section[pair.Key[prefix.Length..]] = pair.Value;
        }

        var root = new Dictionary<string, object?> { [ConfigurationFeatureFlags.SectionName] = section };
        return FromJson(JsonSerializer.Serialize(root));
    }
}
