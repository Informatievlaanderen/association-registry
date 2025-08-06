namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_SubverenigingWerdTeruggezetNaarNietBepaald
{
    [Fact]
    public async ValueTask Then_It_Saves_A_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var rechtspersoonlijkheidScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var subverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario = new SubverenigingWerdTerugGezetNaarNietBepaaldScenario(rechtspersoonlijkheidScenario.VCode);

        var verenigingRepositoryMock = new MultipleVerenigingRepositoryMock(subverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario.GetVerenigingState());
        verenigingRepositoryMock.WithVereniging(rechtspersoonlijkheidScenario.GetVerenigingState());

        var sut = new VoegLidmaatschapToeCommandHandler(verenigingRepositoryMock);

        var command = fixture.Create<VoegLidmaatschapToeCommand>() with
        {
            VCode = subverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario.VCode,
            Lidmaatschap = fixture.Create<VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap>() with
            {
                AndereVereniging = rechtspersoonlijkheidScenario.VCode,
            },
        };

        await sut.Handle(new CommandEnvelope<VoegLidmaatschapToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            subverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario.VCode,
            new LidmaatschapWerdToegevoegd(
                subverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario.VCode,
                new Registratiedata.Lidmaatschap(
                    1,
                    command.Lidmaatschap.AndereVereniging,
                    command.Lidmaatschap.AndereVerenigingNaam,
                    command.Lidmaatschap.Geldigheidsperiode.Van.DateOnly,
                    command.Lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
                    command.Lidmaatschap.Identificatie,
                    command.Lidmaatschap.Beschrijving)));
    }
}
