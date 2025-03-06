namespace AssociationRegistry.Test.Framework;

using System;
using Xunit;

public class IgnoreMagdaTestsFactAttribute : FactAttribute
{
    public IgnoreMagdaTestsFactAttribute()
    {
        const string envVar = "IGNORE_MAGDA_TESTS";
        var env = Environment.GetEnvironmentVariable(envVar);

        if (!bool.TryParse(env, out var ignoreTests))
            return;

        if (ignoreTests)
            Skip = $"Ignored because {envVar} env var is set.";
    }
}

public sealed class IgnoreMagdaTestsTheoryAttribute : TheoryAttribute
{
    public IgnoreMagdaTestsTheoryAttribute()
    {
        const string envVar = "IGNORE_MAGDA_TESTS";
        var env = Environment.GetEnvironmentVariable(envVar);

        if (!bool.TryParse(env, out var ignoreTests))
            return;

        if (ignoreTests)
            Skip = $"Ignored because {envVar} env var is set.";
    }
}
