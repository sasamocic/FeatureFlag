namespace FeatureFlag;

public interface IFeatureFlags
{
    bool IsEnabled(string featureName);
}
