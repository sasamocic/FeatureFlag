namespace FeatureFlag.Tests;

public sealed class ConfigurationFeatureFlagsTests
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("\"true\"", true)]
    [InlineData("\"True\"", true)]
    [InlineData("\"TRUE\"", true)]
    [InlineData("false", false)]
    [InlineData("\"false\"", false)]
    [InlineData("\"FALSE\"", false)]
    public void IsEnabled_parses_boolean_configuration_values(string flagJsonLiteral, bool expected)
    {
        var json = $"{{\"FeatureFlags\":{{\"Flag\":{flagJsonLiteral}}}}}";
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(json));

        Assert.Equal(expected, flags.IsEnabled("Flag"));
    }

    [Fact]
    public void IsEnabled_returns_false_when_key_missing()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson("{}"));

        Assert.False(flags.IsEnabled("Missing"));
    }

    [Fact]
    public void IsEnabled_returns_false_when_feature_flags_section_is_empty_object()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{}}"""));

        Assert.False(flags.IsEnabled("Any"));
    }

    [Fact]
    public void IsEnabled_ignores_singular_FeatureFlag_root_section()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlag":{"X":true}}"""));

        Assert.False(flags.IsEnabled("X"));
    }

    [Fact]
    public void IsEnabled_resolves_feature_names_case_insensitively_for_json_configuration()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"myflag":true}}"""));

        Assert.True(flags.IsEnabled("MYFLAG"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsEnabled_returns_false_for_null_empty_or_whitespace_name(string? featureName)
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"Any":true}}"""));

        Assert.False(flags.IsEnabled(featureName!));
    }

    [Theory]
    [InlineData(@"{""FeatureFlags"":{""Flag"":""maybe""}}")]
    [InlineData(@"{""FeatureFlags"":{""Flag"":""1""}}")]
    [InlineData(@"{""FeatureFlags"":{""Flag"":""0""}}")]
    [InlineData(@"{""FeatureFlags"":{""Flag"":""""}}")]
    [InlineData(@"{""FeatureFlags"":{""Flag"":1}}")]
    public void IsEnabled_returns_false_for_non_boolean_or_empty_value(string json)
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(json));

        Assert.False(flags.IsEnabled("Flag"));
    }

    [Fact]
    public void IsEnabled_returns_false_when_value_is_whitespace_only()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"Flag":"   "}}"""));

        Assert.False(flags.IsEnabled("Flag"));
    }

    [Fact]
    public void IsEnabled_accepts_padded_true_and_false_strings()
    {
        var trueFlags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"Flag":"  true  "}}"""));
        var falseFlags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"Flag":"  false  "}}"""));

        Assert.True(trueFlags.IsEnabled("Flag"));
        Assert.False(falseFlags.IsEnabled("Flag"));
    }

    [Fact]
    public void IsEnabled_treats_colon_in_name_as_nested_configuration_path()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson(
            """{"FeatureFlags":{"A":{"B":true}}}"""));

        Assert.True(flags.IsEnabled("A:B"));
    }

    [Fact]
    public void IFeatureFlags_SectionName_matches_default_configuration_section()
    {
        var flags = new ConfigurationFeatureFlags(ConfigurationTestHelpers.FromJson("{}"));

        Assert.Equal(ConfigurationFeatureFlags.DefaultSectionName, flags.SectionName);
    }

    [Fact]
    public void Constructor_throws_when_configuration_null()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ConfigurationFeatureFlags(null!));

        Assert.Equal("configuration", ex.ParamName);
    }
}
