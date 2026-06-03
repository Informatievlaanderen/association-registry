namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;

public record ActiveerErkenningCommand(string VCode, int ErkenningId)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {ErkenningId}, ");

        return true;
    }
}
