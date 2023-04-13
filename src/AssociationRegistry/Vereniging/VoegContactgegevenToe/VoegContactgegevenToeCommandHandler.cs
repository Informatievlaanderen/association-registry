﻿namespace AssociationRegistry.Vereniging.VoegContactgegevenToe;

using Framework;
using VCodes;

public class VoegContactgegevenToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegContactgegevenToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegContactgegevenToeCommand> envelope)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        vereniging.VoegContactgegevenToe(envelope.Command.Contactgegeven);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
