namespace AssociationRegistry.Test.Common.Extensions;

using FluentAssertions.Json;
using FluentAssertions.Primitives;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json.Linq;

public static class FluentAssertionsExtentions
{
    public static void BeEquivalentJson(this StringAssertions assertion, string json)
    {
        var deserializedContent = JToken.Parse(assertion.Subject);
        var deserializedGoldenMaster = JToken.Parse(json);
        deserializedContent.Should().BeEquivalentTo(deserializedGoldenMaster);
    }
}
