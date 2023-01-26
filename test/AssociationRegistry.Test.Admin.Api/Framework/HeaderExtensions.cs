namespace AssociationRegistry.Test.Admin.Api.Framework;

using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.Net.Http.Headers;

public static class HeaderExtensions
{
    public static void ShouldHaveValidEtagHeader(this HttpResponseHeaders source)
    {
        source.ETag.Should().NotBeNull();
        var etagValues = source.GetValues(HeaderNames.ETag).ToList();
        etagValues.Should().HaveCount(1);
        var etag = etagValues[0];
        etag.Should().StartWith("W/\"").And.EndWith("\"");
        var etagValue = etag.Replace("W/\"", string.Empty).Replace("\"", string.Empty);
        var tryParse = long.TryParse(etagValue, out var etagNumber);
        tryParse.Should().BeTrue();
        etagNumber.Should().BePositive();
    }
}
