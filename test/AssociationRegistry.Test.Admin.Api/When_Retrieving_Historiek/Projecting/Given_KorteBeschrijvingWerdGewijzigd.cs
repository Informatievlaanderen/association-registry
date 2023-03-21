namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteBeschrijvingWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<KorteBeschrijvingWerdGewijzigd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            $"Korte beschrijving werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.KorteBeschrijving}'.");
    }
}
