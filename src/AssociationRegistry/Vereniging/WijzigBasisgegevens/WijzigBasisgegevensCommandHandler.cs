namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Framework;
using VCodes;
using VerenigingsNamen;

public class WijzigBasisgegevensCommandHandler
{
    public async Task<CommandResult> Handle(CommandEnvelope<WijzigBasisgegevensCommand> message, IVerenigingsRepository repository)
    {
        var vereniging = await repository.Load(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        if (message.Command.Naam is { } naam)
            vereniging.WijzigNaam(new VerenigingsNaam(naam));

        if (message.Command.KorteNaam is { } korteNaam)
            vereniging.WijzigKorteNaam(korteNaam);

        var result = await repository.Save(vereniging, message.Metadata);
        return new CommandResult(VCode.Create(message.Command.VCode), result.Sequence, result.Version);
    }
}
