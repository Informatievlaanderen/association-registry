namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Kbo;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_VerenigingVolgensKbo_Invalid_Contactgegevens
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public With_VerenigingVolgensKbo_Invalid_Contactgegevens()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();


        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();
        _verenigingVolgensKbo.Contactgegevens = new ContactgegevensVolgensKbo()
        {
            Email = fixture.Create<string>(),
            Website = fixture.Create<string>(),
            Telefoonnummer = fixture.Create<string>(),
            GSM = fixture.Create<string>(),
        };

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
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                ContactgegevenType.Email.Waarde,
                ContactgegevenTypeVolgensKbo.Email.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Email!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                ContactgegevenType.Website.Waarde,
                ContactgegevenTypeVolgensKbo.Website.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Website!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                ContactgegevenType.Telefoon.Waarde,
                ContactgegevenTypeVolgensKbo.Telefoon.Waarde,
                _verenigingVolgensKbo.Contactgegevens.Telefoonnummer!
            ),
            new ContactgegevenKonNietOvergenomenWordenUitKBO(
                ContactgegevenType.Telefoon.Waarde,
                ContactgegevenTypeVolgensKbo.GSM.Waarde,
                _verenigingVolgensKbo.Contactgegevens.GSM!
            )
        );
    }
}
