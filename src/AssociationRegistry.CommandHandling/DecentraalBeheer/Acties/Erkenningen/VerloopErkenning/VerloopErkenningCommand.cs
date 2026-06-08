namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;

public record VerloopErkenningCommand(string VCode, int ErkenningId)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {ErkenningId}, ");

        return true;
    }
}
