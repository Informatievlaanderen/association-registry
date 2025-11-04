namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Marten;
using Persoonsgegevens;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IDocumentStore _store;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository, IDocumentStore store)
    {
        _repository = verenigingRepository;
        _store = store;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var refId = Guid.NewGuid();

        var vertegenwoordigersPersoonsgegevens = new VertegenwoordigerPersoonsgegevens
        {
            RefId = refId,
            VCode = envelope.Command.VCode,
            VertegenwoordigerId = envelope.Command.Vertegenwoordiger.VertegenwoordigerId,
            Insz = envelope.Command.Vertegenwoordiger.Insz,
            IsPrimair = envelope.Command.Vertegenwoordiger.IsPrimair,
            Roepnaam = envelope.Command.Vertegenwoordiger.Roepnaam,
            Rol = envelope.Command.Vertegenwoordiger.Rol,
            Voornaam = envelope.Command.Vertegenwoordiger.Voornaam,
            Achternaam = envelope.Command.Vertegenwoordiger.Achternaam,
            Email = envelope.Command.Vertegenwoordiger.Email.Waarde,
            Telefoon = envelope.Command.Vertegenwoordiger.Telefoon.Waarde,
            Mobiel = envelope.Command.Vertegenwoordiger.Mobiel.Waarde,
            SocialMedia = envelope.Command.Vertegenwoordiger.SocialMedia.Waarde,
        };

        await using var session = _store.LightweightSession();
        session.Store(vertegenwoordigersPersoonsgegevens);
        await session.SaveChangesAsync(cancellationToken);
        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger, refId);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), vertegenwoordigerId.VertegenwoordigerId, result);
    }
}
