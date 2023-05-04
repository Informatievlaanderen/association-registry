namespace AssociationRegistry.Acties.WijzigVertegenwoordiger;

using Framework;
using Vereniging;

public class WijzigVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigVertegenwoordigerCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigVertegenwoordigerCommand> envelope)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        var (vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair) = envelope.Command.Vertegenwoordiger;
        vereniging.WijzigVertegenwoordiger(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
