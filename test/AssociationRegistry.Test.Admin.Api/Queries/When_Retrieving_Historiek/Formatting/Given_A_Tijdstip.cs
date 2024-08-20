<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Formatting/Given_A_Tijdstip.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Formatting;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Formatting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Formatting/Given_A_Tijdstip.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using FluentAssertions;
using NodaTime;
using Xunit;

public class Given_A_Tijdstip
{
    [Fact]
    public void Then_it_formats_to_zulu_time()
    {
        var instant = Instant.FromDateTimeOffset(new DateTimeOffset(year: 2020, month: 9, day: 9, hour: 12, minute: 30, second: 0,
                                                                    millisecond: 0, TimeSpan.FromHours(2)));

        instant.ToZuluTime().Should().Be("2020-09-09T10:30:00Z");
    }
}
