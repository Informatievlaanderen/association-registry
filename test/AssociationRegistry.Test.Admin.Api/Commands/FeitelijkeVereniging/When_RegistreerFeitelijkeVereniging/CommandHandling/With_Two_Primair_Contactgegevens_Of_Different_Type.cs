namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Events;
using Framework;
using Framework.Fakes;
using Grar;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingCommand _command;
    private readonly RegistreerFeitelijkeVerenigingCommandHandler _commandHandler;
    private readonly IFixture _fixture;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAdminApi()
                                .WithoutWerkingsgebieden();

        _repositoryMock = new VerenigingRepositoryMock();

        _command = _fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Contactgegevens = new[]
            {
                Contactgegeven.CreateFromInitiator(Contactgegeventype.Email, waarde: "test@example.org", _fixture.Create<string>(),
                                                   isPrimair: true),
                Contactgegeven.CreateFromInitiator(Contactgegeventype.Website, waarde: "http://example.org", _fixture.Create<string>(),
                                                   isPrimair: true),
            },
        };

        _vCodeService = new InMemorySequentialVCodeService();

        _commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            _repositoryMock,
            _vCodeService,
            new NoDuplicateVerenigingDetectionService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(_command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerFeitelijkeVerenigingCommandHandler>.Instance);
    }

    public async Task InitializeAsync()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(_command, commandMetadata),
                                     CancellationToken.None);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                Registratiedata.Doelgroep.With(_command.Doelgroep),
                _command.IsUitgeschrevenUitPubliekeDatastroom,
                new[]
                {
                    new Registratiedata.Contactgegeven(
                        ContactgegevenId: 1,
                        Contactgegeventype.Email,
                        _command.Contactgegevens[0].Waarde,
                        _command.Contactgegevens[0].Beschrijving,
                        _command.Contactgegevens[0].IsPrimair
                    ),
                    new Registratiedata.Contactgegeven(
                        ContactgegevenId: 2,
                        Contactgegeventype.Website,
                        _command.Contactgegevens[1].Waarde,
                        _command.Contactgegevens[1].Beschrijving,
                        _command.Contactgegevens[1].IsPrimair
                    ),
                },
                _command.Locaties.Select(
                    (l, index) => Registratiedata.Locatie.With(l) with
                    {
                        LocatieId = index + 1,
                    }).ToArray(),
                _command.Vertegenwoordigers.Select(
                    (v, index) => Registratiedata.Vertegenwoordiger.With(v) with
                    {
                        VertegenwoordigerId = index + 1,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new Registratiedata.HoofdactiviteitVerenigingsloket(
                        h.Code,
                        h.Naam)).ToArray()
            ));
    }
}
