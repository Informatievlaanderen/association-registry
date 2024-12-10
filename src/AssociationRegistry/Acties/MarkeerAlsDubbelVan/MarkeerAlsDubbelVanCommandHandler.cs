namespace AssociationRegistry.Acties.MarkeerAlsDubbelVan;

using Framework;
using Vereniging;
using Vereniging.Exceptions;

public class MarkeerAlsDubbelVanCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;

    public MarkeerAlsDubbelVanCommandHandler(IVerenigingsRepository verenigingsRepository)
    {
        _verenigingsRepository = verenigingsRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingsRepository.Load<Vereniging>(message.Command.VCode, message.Metadata.ExpectedVersion);

        if (await _verenigingsRepository.IsVerwijderd(message.Command.IsDubbelVan))
            throw new VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging();

        if (await _verenigingsRepository.IsDubbel(message.Command.IsDubbelVan))
            throw new VerenigingKanGeenDubbelWordenVanDubbelVereniging();

        vereniging.MarkeerAlsDubbelVan(message.Command.IsDubbelVan);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(message.Command.VCode, result);
    }
}
