namespace AssociationRegistry.Test.Framework;

using AutoFixture;
using System;

public static class AutoFixtureExtensions
{
    public static string CreateStringOfMaxLength(this IFixture fixture, int maxLength)
    {
        var value = fixture.Create<string>();

        return value[..Math.Min(value.Length, maxLength)];
    }
}
