namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using Events;
using Framework.MagdaMocks;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerVerenigingCommandHandler _commandHandler;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly RegistreerVerenigingCommand _command;
    private readonly Fixture _fixture;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAll();
        _repositoryMock = new VerenigingRepositoryMock();
        _command = _fixture.Create<RegistreerVerenigingCommand>() with
        {
            Contactgegevens = new[]
            {
                Contactgegeven.Create(ContactgegevenType.Email, "test@example.org", _fixture.Create<string>(), true),
                Contactgegeven.Create(ContactgegevenType.Website, "http://example.org", _fixture.Create<string>(), true),
            },
        };

        _vCodeService = new InMemorySequentialVCodeService();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            _repositoryMock,
            _vCodeService,
            new MagdaFacadeEchoMock(),
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(_command.Startdatum.Datum!.Value));


    }

    public async Task InitializeAsync()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();
        await _commandHandler.Handle(new CommandEnvelope<RegistreerVerenigingCommand>(_command, commandMetadata), CancellationToken.None);
    }


    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                _command.KboNummer,
                new[]
                {
                    new VerenigingWerdGeregistreerd.Contactgegeven(
                        1,
                        ContactgegevenType.Email,
                        _command.Contactgegevens[0].Waarde,
                        _command.Contactgegevens[0].Beschrijving,
                        _command.Contactgegevens[0].IsPrimair
                    ),
                    new VerenigingWerdGeregistreerd.Contactgegeven(
                        2,
                        ContactgegevenType.Website,
                        _command.Contactgegevens[1].Waarde,
                        _command.Contactgegevens[1].Beschrijving,
                        _command.Contactgegevens[1].IsPrimair
                    ),
                },
                _command.Locaties.Select(VerenigingWerdGeregistreerd.Locatie.With).ToArray(),
                _command.Vertegenwoordigers.Select(
                    v => VerenigingWerdGeregistreerd.Vertegenwoordiger.With(v) with
                    {
                        Voornaam = v.Insz,
                        Achternaam = v.Insz,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(
                        h.Code,
                        h.Beschrijving)).ToArray()));
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
