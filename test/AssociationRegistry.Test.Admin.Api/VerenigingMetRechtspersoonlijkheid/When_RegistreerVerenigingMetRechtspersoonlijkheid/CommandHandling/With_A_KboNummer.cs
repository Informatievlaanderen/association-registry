namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using Kbo;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_KboNummer
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public With_A_KboNummer()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        _command = fixture.Create<RegistreerVerenigingUitKboCommand>();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = new VerenigingVolgensKbo
        {
            KboNummer = _command.KboNummer,
            Type = Verenigingstype.VZW,
            Naam = fixture.Create<string>(),
            KorteNaam = fixture.Create<string>(),
            Startdatum = fixture.Create<DateOnly>(),
        };
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
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.KboNummer,
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum));
    }
}
