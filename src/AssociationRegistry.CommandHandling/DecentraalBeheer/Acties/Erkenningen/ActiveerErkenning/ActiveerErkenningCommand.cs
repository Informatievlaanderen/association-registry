namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record ActiveerErkenningCommand(VCode VCode, int ErkenningId)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        return true;
    }
}
