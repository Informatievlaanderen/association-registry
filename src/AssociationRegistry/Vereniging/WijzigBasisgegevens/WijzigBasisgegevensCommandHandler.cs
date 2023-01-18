namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Framework;
using VCodes;
using VerenigingsNamen;

public class WijzigBasisgegevensCommandHandler
{
    public async Task Handle(CommandEnvelope<WijzigBasisgegevensCommand> message, IVerenigingsRepository repository)
    {
        var vereniging = await repository.Load(VCode.Create(message.Command.VCode));

        vereniging.WijzigNaam(new VerenigingsNaam(message.Command.Naam!));

        await repository.Save(vereniging, message.Metadata);
    }
}
