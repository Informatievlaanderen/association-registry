namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using FluentAssertions;
using Framework;
using Kbo;
using ResultNet;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_VerenigingVolgensKbo_Adres
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public With_An_VerenigingVolgensKbo_Adres()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();


        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();
        _verenigingVolgensKbo.Adres = Result.Success<Adres?>(fixture.Create<Adres>());

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(
                _verenigingVolgensKbo
            ));

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
                _verenigingVolgensKbo.Rechtsvorm.Waarde,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.StartDatum),
            new MaatschappelijkeZetelWerdOvergenomenUitKbo(
                new Registratiedata.Locatie(
                    1,
                    Locatietype.MaatschappelijkeZetelVolgensKbo,
                    false,
                    string.Empty,
                    new Registratiedata.Adres(
                        _verenigingVolgensKbo.Adres.Data!.Straatnaam,
                        _verenigingVolgensKbo.Adres.Data.Huisnummer,
                        _verenigingVolgensKbo.Adres.Data.Busnummer,
                        _verenigingVolgensKbo.Adres.Data.Postcode,
                        _verenigingVolgensKbo.Adres.Data.Gemeente,
                        _verenigingVolgensKbo.Adres.Data.Land
                    ),
                    null)
            )
        );
    }
}
