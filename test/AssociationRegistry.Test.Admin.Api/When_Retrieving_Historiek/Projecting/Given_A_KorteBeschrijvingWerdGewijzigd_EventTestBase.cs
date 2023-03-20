namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using Xunit;

public class Given_A_KorteBeschrijvingWerdGewijzigd_EventTestBase : GivenAnEventTestBase<KorteBeschrijvingWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte beschrijving vereniging werd gewijzigd naar '{Event.Data.KorteBeschrijving}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}
