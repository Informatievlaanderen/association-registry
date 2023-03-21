namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using Xunit;

public class Given_A_KorteNaamWerdGewijzigd : GivenAnEventTestBase<KorteNaamWerdGewijzigd>
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
        => AppendsTheCorrectGebeurtenissen(
            $"Korte naam vereniging werd gewijzigd naar '{Event.Data.KorteNaam}' door {Event.Initiator} op datum {Event.Tijdstip.ToBelgianDateAndTime()}.");
}
