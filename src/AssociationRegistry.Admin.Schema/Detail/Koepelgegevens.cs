namespace AssociationRegistry.Admin.Schema.Detail;

using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

public record Koepelgegevens
{
    public static Koepelgegevens Default => new();

    public string Koepelstatus { get; init; } = DomainKoepelstatus.NietBepaald.Value;
    public bool PrivacyVerklaringGeaccepteerd { get; init; }
}
