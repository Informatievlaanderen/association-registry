namespace AssociationRegistry.Test.Framework;

using AutoFixture;

public static class AutoFixtureExtensions
{
    public static string CreateStringOfMaxLength(this IFixture fixture, int maxLength)
    {
        var value = fixture.Create<string>();
        // Ensure maxLength is non-negative and doesn't exceed the length of the value
        return value.Substring(0, Math.Min(value.Length, maxLength));
    }
}
