namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using ContactGegevens;
using Events;
using Framework.MagdaMocks;
using Hoofdactiviteiten;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Vertegenwoordiger_With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;
    private readonly VerenigingRepositoryMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_A_Vertegenwoordiger_With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        var fixture = new Fixture().CustomizeAll();
        _repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateTime>();
        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Vertegenwoordigers = new[]
            {
                fixture.Create<RegistreerVerenigingCommand.Vertegenwoordiger>() with
                {
                    Contactgegevens = new[]
                    {
                        new RegistreerVerenigingCommand.Contactgegeven(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), true),
                        new RegistreerVerenigingCommand.Contactgegeven(ContactgegevenType.Website, "http://example.org", fixture.Create<string>(), true),
                    },
                },
            },
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _vCodeService = new InMemorySequentialVCodeService();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            _repositoryMock,
            _vCodeService,
            new MagdaFacadeEchoMock(),
            Mock.Of<IDuplicateDetectionService>(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    public async Task InitializeAsync()
        => await _commandHandler.Handle(_commandEnvelope, CancellationToken.None);


    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _commandEnvelope.Command.Naam,
                _commandEnvelope.Command.KorteNaam,
                _commandEnvelope.Command.KorteBeschrijving,
                _commandEnvelope.Command.Startdatum.HasValue ? _commandEnvelope.Command.Startdatum.Value : null,
                _commandEnvelope.Command.KboNumber,
                _commandEnvelope.Command.Contactgegevens.Select(
                    (c, i) => new VerenigingWerdGeregistreerd.Contactgegeven(
                        i+1,
                        c.Type,
                        c.Waarde,
                        c.Omschrijving ?? string.Empty,
                        c.IsPrimair
                    )).ToArray(),
                _commandEnvelope.Command.Locaties.Select(
                    l => new VerenigingWerdGeregistreerd.Locatie(
                        l.Naam,
                        l.Straatnaam,
                        l.Huisnummer,
                        l.Busnummer,
                        l.Postcode,
                        l.Gemeente,
                        l.Land,
                        l.Hoofdlocatie,
                        l.Locatietype
                    )).ToArray(),
                _commandEnvelope.Command.Vertegenwoordigers.Select(
                    v => new VerenigingWerdGeregistreerd.Vertegenwoordiger(
                        v.Insz,
                        v.PrimairContactpersoon,
                        v.Roepnaam,
                        v.Rol,
                        v.Insz,
                        v.Insz,
                        v.Contactgegevens.Select(
                            (c, i) => new VerenigingWerdGeregistreerd.Contactgegeven(
                                i+1,
                                c.Type,
                                c.Waarde,
                                c.Omschrijving ?? string.Empty,
                                c.IsPrimair
                            )).ToArray()
                    )).ToArray(),
                _commandEnvelope.Command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(
                        HoofdactiviteitVerenigingsloket.Create(h).Code,
                        HoofdactiviteitVerenigingsloket.Create(h).Beschrijving
                    )).ToArray()));
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
