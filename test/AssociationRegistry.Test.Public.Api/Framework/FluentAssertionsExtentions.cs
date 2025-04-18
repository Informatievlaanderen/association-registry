﻿namespace AssociationRegistry.Test.Public.Api.Framework;

using FluentAssertions;
using FluentAssertions.Json;
using FluentAssertions.Primitives;
using Nest;
using Newtonsoft.Json.Linq;

public static class FluentAssertionsExtentions
{
    public static void BeEquivalentJson(this StringAssertions assertion, string json)
    {
        var deserializedContent = JToken.Parse(assertion.Subject);
        var deserializedGoldenMaster = JToken.Parse(json);
        deserializedContent.Should().BeEquivalentTo(deserializedGoldenMaster);
    }

    public static void ShouldBeValidIndexResponse(this IndexResponse? indexResponse)
    {
        indexResponse!.IsValid.Should().BeTrue(because: $"Did not expect to have invalid response: '{indexResponse.DebugInformation}'");
    }
}
