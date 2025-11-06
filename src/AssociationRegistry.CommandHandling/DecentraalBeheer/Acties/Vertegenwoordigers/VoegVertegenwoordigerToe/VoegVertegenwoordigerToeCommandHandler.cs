namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Persoonsgegevens;
using Framework;
using Marten;
using Persoonsgegevens;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;

    public VoegVertegenwoordigerToeCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository)
    {
        _repository = verenigingRepository;
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var refId = Guid.NewGuid();

        await _vertegenwoordigerPersoonsgegevensRepository.Save(new VertegenwoordigerPersoonsgegevens(
                                                                    refId,
                                                                    envelope.Command.VCode,
                                                                    envelope.Command.Vertegenwoordiger.VertegenwoordigerId,
                                                                    envelope.Command.Vertegenwoordiger.Insz,
                                                                    envelope.Command.Vertegenwoordiger.IsPrimair,
                                                                    envelope.Command.Vertegenwoordiger.Roepnaam,
                                                                    envelope.Command.Vertegenwoordiger.Rol,
                                                                    envelope.Command.Vertegenwoordiger.Voornaam,
                                                                    envelope.Command.Vertegenwoordiger.Achternaam,
                                                                    envelope.Command.Vertegenwoordiger.Email.Waarde,
                                                                    envelope.Command.Vertegenwoordiger.Telefoon.Waarde,
                                                                    envelope.Command.Vertegenwoordiger.Mobiel.Waarde,
                                                                    envelope.Command.Vertegenwoordiger.SocialMedia.Waarde
                                                                ));

        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger, refId);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), vertegenwoordigerId.VertegenwoordigerId, result);
    }
}
