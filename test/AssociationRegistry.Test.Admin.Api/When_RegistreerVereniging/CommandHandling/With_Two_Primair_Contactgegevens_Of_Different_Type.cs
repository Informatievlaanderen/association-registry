namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Framework.MagdaMocks;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerVerenigingCommand _command;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAll();
        _repositoryMock = new VerenigingRepositoryMock();
        _command = _fixture.Create<RegistreerVerenigingCommand>() with
        {
            Contactgegevens = new[]
            {
                Contactgegeven.Create(ContactgegevenType.Email, "test@example.org", _fixture.Create<string>(), isPrimair: true),
                Contactgegeven.Create(ContactgegevenType.Website, "http://example.org", _fixture.Create<string>(), isPrimair: true),
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

    public Task DisposeAsync()
        => Task.CompletedTask;


    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                VerenigingsType.FeitelijkeVereniging.Code,
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                new[]
                {
                    new FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven(
                        ContactgegevenId: 1,
                        ContactgegevenType.Email,
                        _command.Contactgegevens[0].Waarde,
                        _command.Contactgegevens[0].Beschrijving,
                        _command.Contactgegevens[0].IsPrimair
                    ),
                    new FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven(
                        ContactgegevenId: 2,
                        ContactgegevenType.Website,
                        _command.Contactgegevens[1].Waarde,
                        _command.Contactgegevens[1].Beschrijving,
                        _command.Contactgegevens[1].IsPrimair
                    ),
                },
                _command.Locaties.Select(FeitelijkeVerenigingWerdGeregistreerd.Locatie.With).ToArray(),
                _command.Vertegenwoordigers.Select(
                    (v, index) => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger.With(v) with
                    {
                        VertegenwoordigerId = index + 1,
                        Voornaam = v.Insz,
                        Achternaam = v.Insz,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(
                        h.Code,
                        h.Beschrijving)).ToArray()));
    }
}
