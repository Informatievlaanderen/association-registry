namespace AssociationRegistry.Vereniging.VerwijderContactgegevens;

using Framework;

public class VerwijderContactgegevenCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VerwijderContactgegevenCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VerwijderContactgegevenCommand> envelope)
    {
        throw new NotImplementedException();
    }

}
