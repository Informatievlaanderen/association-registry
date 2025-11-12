namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;
using AssociationRegistry.DecentraalBeheer.Vereniging.TelefoonNummers;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Xunit;

public class Given_A_Vertegenwoordiger_WithKboVereniging :
    WijzigVertegenwoordigerCommandHandlerTestBase<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario>
{
    public VertegenwoordigerWerdToegevoegdVanuitKBO TeWijzigenVertegenwoordiger => Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1;

    public VertegenwoordigerPersoonsgegevensDocument? PersoonsdataBijRegistratie
        => throw new NotImplementedException();//VertegenwoordigerPersoonsgegevensRepositoryMock.FindByRefId(TeWijzigenVertegenwoordiger.RefId);

    protected override WijzigVertegenwoordigerCommand CreateCommand() => new(
        Scenario.VCode,
        new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
            TeWijzigenVertegenwoordiger.VertegenwoordigerId,
            Fixture.Create<string?>(),
            Fixture.Create<string?>(),
            Fixture.Create<Email>(),
            Fixture.Create<TelefoonNummer>(),
            Fixture.Create<TelefoonNummer>(),
            Fixture.Create<SocialMedia>(),
            IsPrimair: false));

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdGewijzigd_Event_Is_Saved_With_The_Next_Id()
        => VerenigingRepositoryMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdGewijzigd(
                VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last(),
                Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1.VertegenwoordigerId,
                Command.Vertegenwoordiger.IsPrimair!.Value)
        );

    [Fact]
    public async ValueTask Then_The_New_Persoonsgegevens_Record_Maintains_Unchangeable_Fields_From_Previous_Event()
    {
        var refId = VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last();
        var actualSaved = await VertegenwoordigerPersoonsgegevensRepositoryMock.Get(refId);

        actualSaved.Insz.Should().Be(Insz.Hydrate(PersoonsdataBijRegistratie.Insz));
        actualSaved.Voornaam.Should().Be(PersoonsdataBijRegistratie.Voornaam);
        actualSaved.Achternaam.Should().Be(PersoonsdataBijRegistratie.Achternaam);
    }

    [Fact]
    public async ValueTask Then_The_New_Persoonsgegevens_Record_Has_Updated_Fields_From_Command()
    {
        var refId = VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last();
        var actualSaved = await VertegenwoordigerPersoonsgegevensRepositoryMock.Get(refId);

        actualSaved.Roepnaam.Should().Be(Command.Vertegenwoordiger.Roepnaam);
        actualSaved.Rol.Should().Be(Command.Vertegenwoordiger.Rol);
        //actualSaved.IsPrimair.Should().Be(Command.Vertegenwoordiger.IsPrimair!.Value);
        actualSaved.Email.Should().Be(Command.Vertegenwoordiger.Email.Waarde);
        actualSaved.Telefoon.Should().Be(Command.Vertegenwoordiger.Telefoon.Waarde);
        actualSaved.Mobiel.Should().Be(Command.Vertegenwoordiger.Mobiel.Waarde);
        actualSaved.SocialMedia.Should().Be(Command.Vertegenwoordiger.SocialMedia.Waarde);
    }

    [Fact]
    public async ValueTask Then_The_New_Persoonsgegevens_Record_Has_Correct_Identifiers()
    {
        var refId = VertegenwoordigerPersoonsgegevensRepositoryMock.SavedRefIds.Last();
        var actualSaved = await VertegenwoordigerPersoonsgegevensRepositoryMock.Get(refId);

        actualSaved.RefId.Should().Be(refId);
        actualSaved.VCode.Should().Be(VCode.Hydrate(Scenario.VCode));

        throw new NotImplementedException();
        // todo: fix
        // actualSaved.VertegenwoordigerId.Should().Be(
        //     Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1);
    }
}
