namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Framework;
using VCodes;
using VerenigingsNamen;

public class WijzigBasisgegevensCommandHandler
{
    public async Task Handle(CommandEnvelope<WijzigBasisgegevensCommand> message, IVerenigingsRepository repository)
    {
        var vereniging = await repository.Load(VCode.Create(message.Command.VCode));

        if (message.Command.Naam is { } naam)
            vereniging.WijzigNaam(new VerenigingsNaam(naam));

        await repository.Save(vereniging, message.Metadata);
    }
}
