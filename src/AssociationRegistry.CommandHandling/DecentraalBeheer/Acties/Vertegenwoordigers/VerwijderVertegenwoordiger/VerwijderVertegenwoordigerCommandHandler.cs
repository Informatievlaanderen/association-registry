namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;
using System.Threading;
using System.Threading.Tasks;

public class VerwijderVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;

    public VerwijderVertegenwoordigerCommandHandler(
        IVerenigingsRepository repository,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository)
    {
        _repository = repository;
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVertegenwoordigerCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();

        await vereniging.VerwijderVertegenwoordiger(message.Command.VertegenwoordigerId, _vertegenwoordigerPersoonsgegevensRepository);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
