namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using Formats;

public record LidmaatschapData(int LidmaatschapId, string AndereVereniging, string Van, string Tot, string Identificatie, string Beschrijving)
{
    public static LidmaatschapData Create(Registratiedata.Lidmaatschap l)
        => new(
            l.LidmaatschapId,
            l.AndereVereniging,
            l.DatumVan.FormatAsBelgianDate(),
            l.DatumTot.FormatAsBelgianDate(),
            l.Identificatie,
            l.Beschrijving
        );
}
