namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Magda;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields
{
    private readonly RegistreerVerenigingCommand _command;
    private readonly string _magdaAchternaam;
    private readonly string _magdaVoornaam;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_All_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();
        Mock<IMagdaFacade> magdaFacade = new();

        var fixture = new Fixture().CustomizeAll();

        _command = fixture.Create<RegistreerVerenigingCommand>();
        var clock = new ClockStub(_command.Startdatum.Datum!.Value);

        _magdaVoornaam = fixture.Create<string>();
        _magdaAchternaam = fixture.Create<string>();
        magdaFacade
            .Setup(facade => facade.GetByInsz(It.IsAny<Insz>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (Insz insz, CancellationToken _) => new MagdaPersoon
                {
                    Insz = insz,
                    Voornaam = _magdaVoornaam,
                    Achternaam = _magdaAchternaam,
                    IsOverleden = false,
                });

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, magdaFacade.Object, new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(_command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                VCode: _vCodeService.GetLast(),
                Naam: _command.Naam,
                KorteNaam: _command.KorteNaam ?? string.Empty,
                KorteBeschrijving: _command.KorteBeschrijving ?? string.Empty,
                Startdatum: _command.Startdatum,
                Contactgegevens: _command.Contactgegevens.Select(
                    (c, i) =>
                        new VerenigingWerdGeregistreerd.Contactgegeven(
                            i + 1,
                            c.Type,
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair
                        )).ToArray(),
                Locaties: _command.Locaties.Select(
                    l =>
                        new VerenigingWerdGeregistreerd.Locatie(
                            l.Naam ?? string.Empty,
                            l.Straatnaam,
                            l.Huisnummer,
                            l.Busnummer ?? string.Empty,
                            l.Postcode,
                            l.Gemeente,
                            l.Land,
                            l.Hoofdlocatie,
                            l.Locatietype)
                ).ToArray(),
                Vertegenwoordigers: _command.Vertegenwoordigers.Select(
                    (v, i) =>
                        new VerenigingWerdGeregistreerd.Vertegenwoordiger(
                            i + 1,
                            v.Insz,
                            v.IsPrimair,
                            v.Roepnaam ?? string.Empty,
                            v.Rol ?? string.Empty,
                            _magdaVoornaam,
                            _magdaAchternaam,
                            v.Email.Waarde,
                            v.Telefoon.Waarde,
                            v.Mobiel.Waarde,
                            v.SocialMedia.Waarde
                        )).ToArray(),
                HoofdactiviteitenVerenigingsloket: _command.HoofdactiviteitenVerenigingsloket.Select(
                    h =>
                        new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)
                ).ToArray()));
    }
}
