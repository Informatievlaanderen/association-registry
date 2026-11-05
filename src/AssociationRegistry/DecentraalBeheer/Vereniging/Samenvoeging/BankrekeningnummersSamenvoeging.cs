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
/// <remarks>
///     De beschrijvingen bevatten geen IBAN, want ze worden mee bewaard in het event.
///     Van een gededupliceerd bankrekeningnummer wordt enkel het id bewaard.
/// </remarks>
public static class BankrekeningnummersSamenvoeging
{
    public static (
        Gegevens.OvergenomenGegeven<Gegevens.Bankrekeningnummer>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Gegevens.NietOvergenomenBankrekeningnummer>[] NietOvergenomen
    ) Merge(Bankrekeningnummers authentiekeVereniging, Bankrekeningnummers dubbeleVereniging)
    {
        var bankrekeningnummers = authentiekeVereniging;
        var resultaat =
            new SamenvoegingResultaat<Gegevens.Bankrekeningnummer, Gegevens.NietOvergenomenBankrekeningnummer>();

        foreach (var bankrekeningnummer in dubbeleVereniging.OrderBy(x => x.BankrekeningnummerId))
        {
            var description = $"Bankrekeningnummer met id {bankrekeningnummer.BankrekeningnummerId}";
            if (bankrekeningnummers.Any(x => Normalize(x.Iban.Value) == Normalize(bankrekeningnummer.Iban.Value)))
            {
                resultaat.Gededupliceerd(
                    description,
                    new Gegevens.NietOvergenomenBankrekeningnummer(bankrekeningnummer.BankrekeningnummerId)
                );

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

            resultaat.Toegevoegd(description, ToEventData(bankrekeningnummer), ToEventData(added));
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
