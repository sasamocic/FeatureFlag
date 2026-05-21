namespace FeatureFlag;

public interface IFeatureFlags
{
    /// <summary>
    /// Configuration section name that holds feature flag keys (default: <c>FeatureFlags</c>).
    /// </summary>
    string SectionName { get; }

    bool IsEnabled(string featureName);
}
