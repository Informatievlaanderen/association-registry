namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Admin.Schema.Persoonsgegevens;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Emails;
using AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;
using AssociationRegistry.DecentraalBeheer.Vereniging.TelefoonNummers;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Events;
using FluentAssertions;
using Xunit;

public class Given_No_Modifications_To_The_Vertegenwoordiger :
    WijzigVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario>
{
    public VertegenwoordigerWerdToegevoegd TeWijzigenVertegenwoordiger => Scenario.VertegenwoordigerWerdToegevoegd;
    public VertegenwoordigerPersoonsgegevensDocument PersoonsdataBijRegistratie
        => VertegenwoordigerPersoonsgegevensRepositoryMock.FindByRefId(TeWijzigenVertegenwoordiger.RefId)!;

    protected override WijzigVertegenwoordigerCommand CreateCommand() =>
        new(
            Scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                TeWijzigenVertegenwoordiger.VertegenwoordigerId,
                PersoonsdataBijRegistratie.Rol,
                PersoonsdataBijRegistratie.Roepnaam,
                Email.Create(PersoonsdataBijRegistratie.Email),
                TelefoonNummer.Create(PersoonsdataBijRegistratie.Telefoon),
                TelefoonNummer.Create(PersoonsdataBijRegistratie.Mobiel),
                SocialMedia.Create(PersoonsdataBijRegistratie.SocialMedia),
                Scenario.VertegenwoordigerWerdToegevoegd.IsPrimair));

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        VerenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public void Then_CommandResult_Has_No_Changes()
    {
        CommandResult.HasChanges().Should().BeFalse();
    }
}
