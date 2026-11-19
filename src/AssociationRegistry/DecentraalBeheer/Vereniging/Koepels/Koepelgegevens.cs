namespace AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;

public record Koepelgegevens(Koepelstatus Koepelstatus, bool PrivacyVerklaringGeaccepteerd)
{
    public static readonly Koepelgegevens Default = new(Koepelstatus.NietBepaald, PrivacyVerklaringGeaccepteerd: false);
}
