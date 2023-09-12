﻿namespace AssociationRegistry.Acties.WijzigMaatschappelijkeZetel;

using Framework;
using JasperFx.Core;
using Vereniging;

public class WijzigMaatschappelijkeZetelCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigMaatschappelijkeZetelCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigMaatschappelijkeZetelCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(envelope.Command.VCode),
                                                                                 envelope.Metadata.ExpectedVersion);

        vereniging.WijzigMaatschappelijkeZetel(envelope.Command.LocatieId, envelope.Command.Naam, envelope.Command.IsPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
