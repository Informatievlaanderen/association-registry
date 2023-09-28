namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Formatting;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using FluentAssertions;
using NodaTime;
using Xunit;

public class Given_A_Tijdstip
{
    [Fact]
    public void Then_it_formats_to_zulu_time()
    {
        var instant = Instant.FromDateTimeOffset(new DateTimeOffset(2020, 9, 9, 12, 30, 0, 0, TimeSpan.FromHours(2)));

        instant.ToZuluTime().Should().Be("2020-09-09T10:30:00Z");
    }
}
