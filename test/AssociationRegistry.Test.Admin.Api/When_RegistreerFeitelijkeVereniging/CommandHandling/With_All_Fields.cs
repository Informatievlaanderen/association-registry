namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
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
    private readonly RegistreerFeitelijkeVerenigingCommand _command;
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

        _command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>();
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
        var commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, magdaFacade.Object, new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(_command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                VerenigingsType.FeitelijkeVereniging.Code,
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                _command.Contactgegevens.Select(
                    (c, i) =>
                        new FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven(
                            i + 1,
                            c.Type,
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair
                        )).ToArray(),
                _command.Locaties.Select(
                    l =>
                        new FeitelijkeVerenigingWerdGeregistreerd.Locatie(
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
                _command.Vertegenwoordigers.Select(
                    (v, i) =>
                        new FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger(
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
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h =>
                        new FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)
                ).ToArray()));
    }
}
