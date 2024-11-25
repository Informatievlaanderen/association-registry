namespace AssociationRegistry.Test.When_Applying.LocatieWerdToegevoegd;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("State")]
public class Given_Locaties_In_State
{
    [Fact]
    public void Then_It_Uses_The_Next_NextId()
    {
        var fixture = new Fixture().CustomizeDomain();

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
            },
        };

        var state = new VerenigingState()
           .Apply(feitelijkeVerenigingWerdGeregistreerd);

        var locatieWerdToegevoegd = new LocatieWerdToegevoegd(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 4,
            });

        var stateAfterApply = state.Apply(locatieWerdToegevoegd);

        stateAfterApply.Locaties.NextId.Should().Be(5);
    }
}
