﻿namespace AssociationRegistry.Vereniging.WijzigContactgegeven;

using Framework;
using VCodes;

public class WijzigContactgegevenCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigContactgegevenCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigContactgegevenCommand> envelope)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        var (contacgegevenId, waarde, omschrijving, isPrimair) = envelope.Command.Contactgegeven;
        vereniging.WijzigContactgegeven(contacgegevenId, waarde, omschrijving, isPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
