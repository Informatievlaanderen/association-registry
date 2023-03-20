namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using Xunit;

public class Given_A_StartdatumWerdGewijzigd_EventTestBase : GivenAnEventTestBase<StartdatumWerdGewijzigd>
{
    [Fact]
    public void Test()
        => AppendsTheCorrectGebeurtenissen(
            $"Startdatum vereniging werd gewijzigd naar '{Event.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}