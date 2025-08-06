namespace AssociationRegistry.Test.KboSync;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using EventStore;
using Kbo;
using KboSyncLambda.SyncKbo;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Notifications;
using ResultNet;
using Vereniging;

public class SyncKboCommandHandlerBuilder
{
    private readonly Fixture _fixture;
    private Mock<IMagdaRegistreerInschrijvingService> _magdaRegistreerInschrijvingService;
    private Mock<IMagdaGeefVerenigingService> _magdaGeefVerenigingService;
    private Mock<IVerenigingsRepository> _verenigingsRepository;
    private KboNummer? _kboNummer;

    public SyncKboCommandHandler SyncKboCommandHandler;

    public SyncKboCommandHandlerBuilder()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _magdaRegistreerInschrijvingService = new Mock<IMagdaRegistreerInschrijvingService>();
        _magdaGeefVerenigingService = new Mock<IMagdaGeefVerenigingService>();
        _verenigingsRepository = new Mock<IVerenigingsRepository>();
        _kboNummer = _fixture.Create<KboNummer>();
    }

    public SyncKboCommandHandlerBuilder MetBestaandeVereniging()
    {
        _verenigingsRepository.Setup(x => x.Exists(_kboNummer))
                              .Returns(Task.FromResult(true));

        return this;
    }

    public SyncKboCommandHandlerBuilder MetNietBestaandeVereniging()
    {
        _verenigingsRepository.Setup(x => x.Exists(_kboNummer))
                              .Returns(Task.FromResult(false));

        return this;
    }

    public SyncKboCommandHandlerBuilder MetGeldigeVerenigingVolgensMagda()
    {
        _magdaGeefVerenigingService
           .Setup(x => x.GeefSyncVereniging(KboNummer.Create(_kboNummer),
                                            It.IsAny<CommandMetadata>(),
                                            It.IsAny<CancellationToken>()))
           .ReturnsAsync(VerenigingVolgensKboResult.GeldigeVereniging(_fixture.Create<VerenigingVolgensKbo>() with
            {
                KboNummer = _kboNummer,
            }));

        return this;
    }

    public SyncKboCommandHandlerBuilder MetGeenGeldigeVerenigingVolgensMagda()
    {
        _magdaGeefVerenigingService
           .Setup(x => x.GeefSyncVereniging(KboNummer.Create(_kboNummer),
                                            It.IsAny<CommandMetadata>(),
                                            It.IsAny<CancellationToken>()))
           .ReturnsAsync(VerenigingVolgensKboResult.GeenGeldigeVereniging);

        return this;
    }

    public SyncKboCommandHandlerBuilder MetFoutBijVerenigingOphalenBijMagda()
    {
        _magdaGeefVerenigingService
           .Setup(x => x.GeefSyncVereniging(KboNummer.Create(_kboNummer),
                                            It.IsAny<CommandMetadata>(),
                                            It.IsAny<CancellationToken>()))
           .ThrowsAsync(new Exception());

        return this;
    }

    public SyncKboCommandHandlerBuilder MetFoutBijLadenVereniging()
    {
        _verenigingsRepository.Setup(x => x.Load(KboNummer.Create(_kboNummer), It.IsAny<CommandMetadata>()))
                              .ThrowsAsync(new Exception());
        return this;
    }

    public SyncKboCommandHandlerBuilder MetVerenigingUitVerenigingsregister()
    {
        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(_fixture.Create<VCode>(), _fixture.Create<VerenigingVolgensKbo>() with
        {
            KboNummer = _kboNummer,
        });

        _verenigingsRepository.Setup(x => x.Load(KboNummer.Create(_kboNummer), It.IsAny<CommandMetadata>()))
                              .ReturnsAsync(vereniging);
        return this;
    }

    public SyncKboCommandHandlerBuilder MetFoutBijHetRegistrerenVanEenInschrijvingBijMagda()
    {
        _magdaRegistreerInschrijvingService.Setup(x => x.RegistreerInschrijving(KboNummer.Create(_kboNummer), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                                           .ReturnsAsync(Result.Failure());
        return this;
    }

    public SyncKboCommandHandlerBuilder MetSuccesBijHetRegistrerenVanEenInschrijvingBijMagda()
    {
        _magdaRegistreerInschrijvingService.Setup(x => x.RegistreerInschrijving(KboNummer.Create(_kboNummer), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                                           .ReturnsAsync(Result.Success);
        return this;
    }

    public SyncKboCommandHandlerBuilder MetSuccesvolOpgeslagenVereniging()
    {
        _verenigingsRepository.Setup(x => x.Save(It.IsAny<VerenigingMetRechtspersoonlijkheid>(), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(_fixture.Create<StreamActionResult>());
        return this;
    }



    public SyncKboCommandHandler Build()
        => new(_magdaRegistreerInschrijvingService.Object, _magdaGeefVerenigingService.Object,
               Mock.Of<INotifier>(), Mock.Of<ILogger<SyncKboCommandHandler>>());

    public async Task<CommandResult?> Handle()
        => await Build().Handle(
            new CommandEnvelope<SyncKboCommand>(new SyncKboCommand(_kboNummer), _fixture.Create<CommandMetadata>()),
            _verenigingsRepository.Object);
}
