namespace AssociationRegistry.Vereniging.WijzigContactgegeven;

using VCodes;

public record WijzigContactgegevenCommand(VCode VCode, WijzigContactgegevenCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Waarde, string? Omschrijving, bool? IsPrimair);
}
