namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using Xunit;

public class Given_StartdatumWerdGewijzigd_With_Null : GivenAnEventTestBase<StartdatumWerdGewijzigd>
{
    public Given_StartdatumWerdGewijzigd_With_Null()
    {
        Event.Data = Event.Data with { Startdatum = null };
    }

    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar '' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}
