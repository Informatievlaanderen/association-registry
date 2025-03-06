namespace AssociationRegistry.Test.Acm.Api.Framework;

using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Net.Http.Headers;

public static class HeaderExtensions
{
    public static void ShouldHaveValidEtagHeader(this HttpResponseHeaders source)
    {
        source.ETag.Should().NotBeNull();
        var etagValues = source.GetValues(HeaderNames.ETag).ToList();
        etagValues.Should().HaveCount(1);
        var etag = etagValues[0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
        var etagValue = etag.Replace(oldValue: "W/\"", string.Empty).Replace(oldValue: "\"", string.Empty);
        var tryParse = long.TryParse(etagValue, out var etagNumber);
        tryParse.Should().BeTrue();
        etagNumber.Should().BePositive();
    }
}
