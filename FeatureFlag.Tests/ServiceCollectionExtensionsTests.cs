using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlag.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFeatureFlags_registers_IFeatureFlags_singleton_using_same_instance()
    {
        var config = ConfigurationTestHelpers.FromKeyValue(
            new KeyValuePair<string, string?>($"{ConfigurationFeatureFlags.DefaultSectionName}:X", "true"));

        var services = new ServiceCollection();
        services.AddFeatureFlags(config);
        using var provider = services.BuildServiceProvider();

        var a = provider.GetRequiredService<IFeatureFlags>();
        var b = provider.GetRequiredService<IFeatureFlags>();

        Assert.Same(a, b);
        Assert.Equal(ConfigurationFeatureFlags.DefaultSectionName, a.SectionName);
        Assert.True(a.IsEnabled("X"));
    }

    [Fact]
    public void AddFeatureFlags_returns_same_service_collection_for_chaining()
    {
        var services = new ServiceCollection();

        var returned = services.AddFeatureFlags(ConfigurationTestHelpers.FromJson("""{"FeatureFlags":{}}"""));

        Assert.Same(services, returned);
    }

    [Fact]
    public void AddFeatureFlags_throws_when_services_null()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddFeatureFlags(null!, new ConfigurationBuilder().Build()));

        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void AddFeatureFlags_throws_when_configuration_null()
    {
        var services = new ServiceCollection();

        var ex = Assert.Throws<ArgumentNullException>(() => services.AddFeatureFlags(null!));

        Assert.Equal("configuration", ex.ParamName);
    }
}
