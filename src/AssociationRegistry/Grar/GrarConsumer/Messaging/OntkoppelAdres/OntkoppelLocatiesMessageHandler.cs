namespace AssociationRegistry.Grar.GrarConsumer.Messaging.OntkoppelAdres;

using Framework;
using Vereniging;
using DecentraalBeheer.Vereniging;

public class OntkoppelLocatiesMessageHandler
{
    private readonly IVerenigingsRepository _repository;

    public OntkoppelLocatiesMessageHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(OntkoppelLocatiesMessage message, CancellationToken cancellationToken)
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode), metadata, allowDubbeleVereniging: true);

        foreach (var teOntkoppelenLocatieId in message.TeOntkoppelenLocatieIds)
        {
            vereniging.OntkoppelLocatie(teOntkoppelenLocatieId);
        }

        await _repository.Save(vereniging, metadata with { ExpectedVersion = vereniging.Version }, cancellationToken);
    }
}
