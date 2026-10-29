namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Scheduled.Host.DubbelBeheer;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

/// <summary>
///     De message bus wordt vervangen: het markeren en aanvaarden zelf wordt getest bij de command handler.
///     De event store is echt, voor de controle of een rij al verwerkt werd.
/// </summary>
[Collection(nameof(DubbelImportCollection))]
public class DubbelImportLineProcessorTests : IAsyncLifetime
{
    private const string Dubbele = "V0004001";
    private const string Authentieke = "V0004002";
    private const string AndereAuthentieke = "V0004003";

    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    private DocumentStore _store = null!;
    private Mock<IMessageBus> _messageBus = null!;
    private DubbelImportLineProcessor _processor = null!;

    public async ValueTask InitializeAsync()
    {
        _store = await TestDocumentStoreFactory.CreateAsync(nameof(DubbelImportLineProcessorTests));
        _messageBus = new Mock<IMessageBus>();

        _processor = new DubbelImportLineProcessor(
            _messageBus.Object,
            _store,
            NullLogger<DubbelImportLineProcessor>.Instance
        );

        await Append(Dubbele, Geregistreerd(Dubbele));
        await Append(Authentieke, Geregistreerd(Authentieke));
    }

    public async ValueTask DisposeAsync() => await _store.DisposeAsync();

    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd Geregistreerd(string vCode) =>
        _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = vCode,
        };

    private async Task Append(string vCode, params object[] events)
    {
        await using var session = _store.LightweightSession();
        session.Events.Append(vCode, events);
        await session.SaveChangesAsync();
    }

    private void MarkeerAlsDubbelVanThrows(Exception exception) =>
        _messageBus
            .Setup(x =>
                x.InvokeAsync<CommandResult>(
                    It.IsAny<CommandEnvelope<MarkeerAlsDubbelVanCommand>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan?>()
                )
            )
            .ThrowsAsync(exception);

    private Task<DubbelImportLineResult> Process(string dubbeleVCode = Dubbele) =>
        _processor.Process(
            new DubbelImportDocument
            {
                LineNumber = 1,
                DubbeleVCode = dubbeleVCode,
                AuthentiekeVCode = Authentieke,
            },
            CancellationToken.None
        );

    private void VerifyMarkeerAlsDubbelVan(Times times) =>
        _messageBus.Verify(
            x =>
                x.InvokeAsync<CommandResult>(
                    It.Is<CommandEnvelope<MarkeerAlsDubbelVanCommand>>(envelope =>
                        envelope.Command.VCode.Value == Dubbele
                        && envelope.Command.VCodeAuthentiekeVereniging.Value == Authentieke
                    ),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan?>()
                ),
            times
        );

    [Fact]
    public async Task Een_gemarkeerde_dubbele_vereniging_is_verwerkt()
    {
        (await Process()).Should().Be(DubbelImportLineResult.Processed);
        VerifyMarkeerAlsDubbelVan(Times.Once());
    }

    [Fact]
    public async Task Een_al_gemarkeerde_dubbele_vereniging_wordt_niet_opnieuw_gemarkeerd()
    {
        await Append(Dubbele, new VerenigingWerdGemarkeerdAlsDubbelVan(Dubbele, Authentieke));

        (await Process()).Should().Be(DubbelImportLineResult.Processed);
        VerifyMarkeerAlsDubbelVan(Times.Never());
    }

    [Fact]
    public async Task Een_dubbele_vereniging_van_een_andere_authentieke_vereniging_wordt_gemarkeerd()
    {
        await Append(AndereAuthentieke, Geregistreerd(AndereAuthentieke));
        await Append(Dubbele, new VerenigingWerdGemarkeerdAlsDubbelVan(Dubbele, AndereAuthentieke));

        await Process();

        VerifyMarkeerAlsDubbelVan(Times.Once());
    }

    [Fact]
    public async Task Een_domeinfout_bij_het_markeren_faalt_met_de_reden()
    {
        var exception = new VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel();
        MarkeerAlsDubbelVanThrows(exception);

        (await Process()).Should().Be(DubbelImportLineResult.Failed(exception.Message));
    }

    [Fact]
    public async Task Een_andere_fout_bij_het_markeren_faalt_met_de_reden()
    {
        var exception = new UnexpectedAggregateVersionException();
        MarkeerAlsDubbelVanThrows(exception);

        (await Process()).Should().Be(DubbelImportLineResult.Failed(exception.Message));
    }

    [Fact]
    public async Task Een_ongeldige_vcode_faalt()
    {
        var result = await Process(dubbeleVCode: "geen vcode");

        result.Status.Should().Be(DubbelImportLineStatus.Failed);
        _messageBus.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Bij_annulering_faalt_de_rij_niet()
    {
        MarkeerAlsDubbelVanThrows(new OperationCanceledException());

        var act = () => Process();

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
