namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;

public record UpdateInStopzettingCommand(string VCode, bool InStopzetting)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"InStopzetting = {InStopzetting}, ");

        return true;
    }
}
