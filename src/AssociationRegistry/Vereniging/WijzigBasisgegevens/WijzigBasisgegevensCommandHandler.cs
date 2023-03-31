namespace AssociationRegistry.Vereniging.WijzigBasisgegevens;

using Framework;
using Primitives;
using Startdatums;
using VCodes;
using VerenigingsNamen;

public class WijzigBasisgegevensCommandHandler
{
    private readonly IClock _clock;

    public WijzigBasisgegevensCommandHandler(IClock clock)
    {
        _clock = clock;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigBasisgegevensCommand> message, IVerenigingsRepository repository)
    {
        var vereniging = await repository.Load(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        HandleNaam(vereniging, message.Command.Naam);
        HandleKorteNaam(vereniging, message.Command.KorteNaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleStartdatum(vereniging, message.Command.Startdatum);

        var result = await repository.Save(vereniging, message.Metadata);
        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

    private void HandleStartdatum(Vereniging vereniging, NullOrEmpty<DateOnly> startdatum)
    {
        if (startdatum.IsNull) return;
        vereniging.WijzigStartdatum(Startdatum.Create(_clock, startdatum));
    }

    private static void HandleKorteBeschrijving(Vereniging vereniging, string? korteBeschrijving)
    {
        if (korteBeschrijving is null) return;
        vereniging.WijzigKorteBeschrijving(korteBeschrijving);
    }

    private static void HandleNaam(Vereniging vereniging, string? naam)
    {
        if (naam is null) return;
        vereniging.WijzigNaam(new VerenigingsNaam(naam));
    }

    private static void HandleKorteNaam(Vereniging vereniging, string? korteNaam)
    {
        if (korteNaam is null) return;
        vereniging.WijzigKorteNaam(korteNaam);
    }
}
