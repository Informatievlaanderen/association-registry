namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using AssociationRegistry.Vereniging.Bronnen;
using Bankrekeningen;
using Events;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Bankrekeningnummers van de dubbele vereniging worden enkel toegevoegd als het IBAN nog niet bestaat op de
///     authentieke vereniging. Bestaande bankrekeningnummers worden niet overschreven; titularissen, doel en
///     bevestigingen worden enkel overgenomen bij een nieuw toegevoegd bankrekeningnummer.
/// </summary>
/// <remarks>De beschrijvingen bevatten geen IBAN, want ze worden mee bewaard in het event.</remarks>
public static class BankrekeningnummersSamenvoeging
{
    public static (
        Gegevens.OvergenomenGegeven<Gegevens.Bankrekeningnummer>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Gegevens.Bankrekeningnummer>[] NietOvergenomen
    ) Merge(Bankrekeningnummers authentiekeVereniging, Bankrekeningnummers dubbeleVereniging)
    {
        var bankrekeningnummers = authentiekeVereniging;
        var resultaat = new SamenvoegingResultaat<Gegevens.Bankrekeningnummer>();

        foreach (var bankrekeningnummer in dubbeleVereniging.OrderBy(x => x.BankrekeningnummerId))
        {
            var description = $"Bankrekeningnummer met id {bankrekeningnummer.BankrekeningnummerId}";
            var original = ToEventData(bankrekeningnummer);

            if (bankrekeningnummers.Any(x => Normalize(x.Iban.Value) == Normalize(bankrekeningnummer.Iban.Value)))
            {
                resultaat.Gededupliceerd(description, original);

                continue;
            }

            var added = bankrekeningnummers.VoegToe(
                new ToeTevoegenBankrekeningnummer
                {
                    Iban = bankrekeningnummer.Iban,
                    Doel = bankrekeningnummer.Doel,
                    Titularissen = bankrekeningnummer.Titularissen,
                }
            ) with
            {
                BevestigdDoor = bankrekeningnummer.BevestigdDoor,
                Bron = Bron.Initiator,
            };
            bankrekeningnummers = bankrekeningnummers.Hydrate(bankrekeningnummers.Append(added));

            resultaat.Toegevoegd(description, original, ToEventData(added));
        }

        return resultaat.Build();
    }

    private static string Normalize(string iban) =>
        iban.Replace(" ", string.Empty).Replace(".", string.Empty).ToUpperInvariant();

    private static Gegevens.Bankrekeningnummer ToEventData(Bankrekeningnummer bankrekeningnummer) =>
        new(
            bankrekeningnummer.BankrekeningnummerId,
            bankrekeningnummer.Iban.Value,
            bankrekeningnummer.Doel,
            bankrekeningnummer.Titularissen.Value,
            (bankrekeningnummer.BevestigdDoor ?? [])
                .Select(x => new Registratiedata.GegevensInitiator(x.OvoCode, x.Naam))
                .ToArray()
        );
}
