namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Events;
using FluentAssertions;
using Xunit;

public class With_A_Known_VertegenwoordigerId : VerwijderVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario>
{
    public VertegenwoordigerWerdToegevoegd TeVerwijderenVertegenwoordiger => Scenario.VertegenwoordigerWerdToegevoegd;

    public VertegenwoordigerPersoonsgegevensDocument? PersoonsdataBijRegistratie
        => VertegenwoordigerPersoonsgegevensRepositoryMock.FindByRefId(TeVerwijderenVertegenwoordiger.RefId);

    [Fact]
    public async Task Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        var refId = VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last();
        VerenigingRepositoryMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdVerwijderd(
                refId,
                Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId)
        );
    }

    protected override VerwijderVertegenwoordigerCommand CreateCommand()
        => new(Scenario.VCode, Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId);

    [Fact]
    public async ValueTask Then_The_New_Persoonsgegevens_Record_Maintains_Unchangeable_Fields_From_Previous_Event()
    {
        var refId = VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last();
        var actualSaved = await VertegenwoordigerPersoonsgegevensRepositoryMock.Get(refId);

        actualSaved.RefId.Should().Be(refId);
        actualSaved.Insz.Should().Be(Insz.Hydrate(PersoonsdataBijRegistratie.Insz));
        actualSaved.VCode.Should().Be(VCode.Hydrate(Scenario.VCode));
        actualSaved.VertegenwoordigerId.Should().Be(PersoonsdataBijRegistratie.VertegenwoordigerId);
        actualSaved.Voornaam.Waarde.Should().Be(PersoonsdataBijRegistratie.Voornaam);
        actualSaved.Achternaam.Waarde.Should().Be(PersoonsdataBijRegistratie.Achternaam);
        actualSaved.Roepnaam.Should().Be(actualSaved.Roepnaam);
        actualSaved.Rol.Should().Be(actualSaved.Rol);
        actualSaved.Email.Should().Be(actualSaved.Email);
        actualSaved.Telefoon.Should().Be(actualSaved.Telefoon);
        actualSaved.Mobiel.Should().Be(actualSaved.Mobiel);
        actualSaved.SocialMedia.Should().Be(actualSaved.SocialMedia);
    }
}
