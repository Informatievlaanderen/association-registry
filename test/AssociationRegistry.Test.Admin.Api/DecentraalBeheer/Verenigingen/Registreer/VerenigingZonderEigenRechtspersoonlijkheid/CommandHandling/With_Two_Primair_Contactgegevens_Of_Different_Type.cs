namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler _commandHandler;
    private readonly IFixture _fixture;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAdminApi()
                                .WithoutWerkingsgebieden();

        _repositoryMock = new VerenigingRepositoryMock();

        _command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
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

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            _repositoryMock,
            _vCodeService,
            new NoDuplicateVerenigingDetectionService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(_command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);
    }

    public async Task InitializeAsync()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(_command, commandMetadata),
                                     CancellationToken.None);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                EventFactory.Doelgroep(_command.Doelgroep),
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
                    (l, index) => EventFactory.Locatie(l) with
                    {
                        LocatieId = index + 1,
                    }).ToArray(),
                _command.Vertegenwoordigers.Select(
                    (v, index) => EventFactory.Vertegenwoordiger(v) with
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
