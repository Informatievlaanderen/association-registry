namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Framework;
using AutoFixture;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_All_Fields
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly RegistreerVerenigingCommand _command;
    private readonly string _magdaVoornaam;
    private readonly string _magdaAchternaam;

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
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                _command.KboNummer,
                _command.Contactgegevens.Select(
                    (c, i) =>
                        new VerenigingWerdGeregistreerd.Contactgegeven(
                            i + 1,
                            c.Type,
                            c.Waarde,
                            c.Beschrijving,
                            c.IsPrimair
                        )).ToArray(),
                _command.Locaties.Select(
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
                _command.Vertegenwoordigers.Select(
                    v =>
                        new VerenigingWerdGeregistreerd.Vertegenwoordiger(
                            v.Insz,
                            v.PrimairContactpersoon,
                            v.Roepnaam ?? string.Empty,
                            v.Rol ?? string.Empty,
                            _magdaVoornaam,
                            _magdaAchternaam,
                            v.Email.Waarde,
                            v.TelefoonNummer.Waarde,
                            v.Mobiel.Waarde,
                            v.SocialMedia.Waarde
                        )).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h =>
                        new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(h.Code, h.Beschrijving)
                ).ToArray()));
    }
}
