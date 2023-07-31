namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using Fakes;
using Framework;
using AutoFixture;
using Kbo;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_KboNummer
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_A_KboNummer()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        _command = fixture.Create<RegistreerVerenigingUitKboCommand>();

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingUitKboCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService(
                new VerenigingVolgensKbo { KboNummer = _command.KboNummer, Rechtsvorm = Rechtsvorm.VZW }
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
                "VZW",
                $"VZW {_command.KboNummer}",
                string.Empty,
                null));
    }
}
