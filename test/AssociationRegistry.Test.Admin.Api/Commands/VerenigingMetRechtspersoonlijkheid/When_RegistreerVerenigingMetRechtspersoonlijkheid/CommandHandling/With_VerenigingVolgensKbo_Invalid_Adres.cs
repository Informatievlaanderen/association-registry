namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using AssociationRegistry.Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Framework.Fakes;
using Microsoft.Extensions.Logging;
using ResultNet;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_VerenigingVolgensKbo_Invalid_Adres
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;
    private readonly LoggerFactory _loggerFactory;

    public With_VerenigingVolgensKbo_Invalid_Adres()
    {
        _loggerFactory = new LoggerFactory();
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();

        _verenigingVolgensKbo.Adres = new AdresVolgensKbo
        {
            Straatnaam = fixture.Create<string>(),
            Huisnummer = null,
            Busnummer = null,
            Postcode = fixture.Create<string>(),
            Gemeente = null,
            Land = null,
        };

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandlerLogger = _loggerFactory.CreateLogger<RegistreerVerenigingUitKboCommandHandler>();

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberFoundServiceMock(_verenigingVolgensKbo),
            new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
            commandHandlerLogger
        );

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.KboNummer,
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum),
            new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
                _verenigingVolgensKbo.Adres.Straatnaam ?? string.Empty,
                _verenigingVolgensKbo.Adres.Huisnummer ?? string.Empty,
                _verenigingVolgensKbo.Adres.Busnummer ?? string.Empty,
                _verenigingVolgensKbo.Adres.Postcode ?? string.Empty,
                _verenigingVolgensKbo.Adres.Gemeente ?? string.Empty,
                _verenigingVolgensKbo.Adres.Land ?? string.Empty
            ),
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        );
    }
}
