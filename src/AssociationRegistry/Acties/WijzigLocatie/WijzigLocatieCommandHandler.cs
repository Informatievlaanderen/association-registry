namespace AssociationRegistry.Acties.WijzigLocatie;

using Framework;
using Vereniging;
using WijzigContactgegeven;

public class WijzigLocatieCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigLocatieCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigLocatieCommand> envelope, CancellationToken cancellationToken = default)
    {
        return null;
    }
}
