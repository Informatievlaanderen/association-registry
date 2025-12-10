namespace AssociationRegistry.Integrations.Magda.Persoon.Validation;

public record PersoonUitzonderingType(string Identificatie, string Beschrijving)
{
    public static readonly PersoonUitzonderingType Fout30001 = new("30001", "Geen gegevens gevonden voor de vraag");
    public static readonly PersoonUitzonderingType Fout30002 = new("30002", "Het INSZ is geannuleerd");
    public static readonly PersoonUitzonderingType Fout30003 = new("30003", "Onbestaand INSZ");
    public static readonly PersoonUitzonderingType Fout30004 = new("30004", "Het INSZ-nummer is vervangen door een ander INSZ-nummer");
    public static readonly PersoonUitzonderingType Fout13202 = new("13202", "Gegevens niet opvraagbaar, geen persoonsdossier geregistreerd");

    public static readonly PersoonUitzonderingType[] FoutcodesVeroorzaaktDoorGebruiker = [Fout30001, Fout30002, Fout30003, Fout30004, Fout13202];
    public static readonly string[] FoutcodesVeroorzaaktDoorGebruikerIdentificaties = FoutcodesVeroorzaaktDoorGebruiker.Select(x => x.Identificatie).ToArray();
}
