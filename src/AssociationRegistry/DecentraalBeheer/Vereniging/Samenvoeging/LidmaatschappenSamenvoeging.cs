namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Events;
using Events.Factories;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Hoe de vereniging waarbij een lidmaatschap hoort er op het moment van de samenvoeging voor staat.
/// </summary>
public enum AndereVerenigingStatus
{
    Actief,
    Verwijderd,
    Dubbel,
    Gestopt,
}

/// <summary>
///     Lidmaatschappen van de dubbele vereniging worden samengevoegd met die van de authentieke vereniging.
///     Niet overgenomen wordt een lidmaatschap
///     <list type="bullet">
///         <item>bij een verwijderde, dubbele of gestopte vereniging,</item>
///         <item>bij de authentieke vereniging zelf,</item>
///         <item>bij een vereniging waarvan de authentieke vereniging subvereniging is,</item>
///         <item>dat overlapt met meer dan één lidmaatschap van de authentieke vereniging bij dezelfde vereniging.</item>
///     </list>
///     Overlapt het met één lidmaatschap van de authentieke vereniging bij dezelfde vereniging, dan is het hetzelfde
///     lidmaatschap: de geldigheidsperiode wordt verruimd tot de vroegste startdatum en de laatste einddatum.
///     Valt de periode al binnen die van de authentieke vereniging, dan wordt het lidmaatschap gededupliceerd.
///     Identificatie en beschrijving van de authentieke vereniging blijven behouden.
///     Een lidmaatschap bij dezelfde vereniging zonder overlap is een ander lidmaatschap en wordt toegevoegd.
/// </summary>
public static class LidmaatschappenSamenvoeging
{
    public const string RedenVerwijderdeVereniging = "De vereniging waarbij het lidmaatschap hoort, werd verwijderd.";

    public const string RedenDubbeleVereniging =
        "De vereniging waarbij het lidmaatschap hoort, werd gemarkeerd als dubbel.";

    public const string RedenGestopteVereniging = "De vereniging waarbij het lidmaatschap hoort, werd gestopt.";

    public const string RedenEigenVereniging = "Het lidmaatschap verwijst naar de authentieke vereniging zelf.";

    public const string RedenSubvereniging =
        "De authentieke vereniging is subvereniging van de vereniging waarbij het lidmaatschap hoort.";

    public const string RedenOverlapMetMeerdere =
        "Het lidmaatschap overlapt met meerdere lidmaatschappen van de authentieke vereniging bij dezelfde vereniging.";

    /// <param name="statusAndereVerenigingen">
    ///     De status van de verenigingen waarbij de dubbele vereniging een lidmaatschap heeft. Ontbreekt een
    ///     vereniging, dan wordt ze als actief beschouwd.
    /// </param>
    public static (
        Gegevens.OvergenomenGegeven<Registratiedata.Lidmaatschap>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<Registratiedata.Lidmaatschap>[] NietOvergenomen
    ) Merge(
        VerenigingState authentiekeVereniging,
        Lidmaatschappen dubbeleVereniging,
        IReadOnlyDictionary<VCode, AndereVerenigingStatus> statusAndereVerenigingen
    )
    {
        var lidmaatschappen = authentiekeVereniging.Lidmaatschappen;
        var resultaat = new SamenvoegingResultaat<Registratiedata.Lidmaatschap>();

        foreach (var lidmaatschap in dubbeleVereniging.OrderBy(x => (int)x.LidmaatschapId))
        {
            var description = Describe(lidmaatschap);
            var original = EventFactory.Lidmaatschap(lidmaatschap);

            var reden = RedenNietOvergenomen(authentiekeVereniging, lidmaatschap, statusAndereVerenigingen);

            if (reden is not null)
            {
                resultaat.NietOvergenomen(description, original, reden);

                continue;
            }

            var overlappend = lidmaatschappen
                .Where(x => x.AndereVereniging == lidmaatschap.AndereVereniging)
                .Where(x => x.Geldigheidsperiode.OverlapsWith(lidmaatschap.Geldigheidsperiode))
                .ToArray();

            if (overlappend.Length > 1)
            {
                resultaat.NietOvergenomen(description, original, RedenOverlapMetMeerdere);

                continue;
            }

            if (overlappend.Length == 1)
            {
                var bestaand = overlappend[0];
                var verruimd = bestaand with
                {
                    Geldigheidsperiode = Verruim(bestaand.Geldigheidsperiode, lidmaatschap.Geldigheidsperiode),
                };

                if (HeeftDezelfdeGeldigheidsperiode(bestaand, verruimd))
                {
                    resultaat.Gededupliceerd(description, original);

                    continue;
                }

                lidmaatschappen = lidmaatschappen.Hydrate(lidmaatschappen.Without(bestaand).Append(verruimd));

                resultaat.Samengevoegd(
                    $"{description} werd samengevoegd met lidmaatschap met id {(int)bestaand.LidmaatschapId}; de geldigheidsperiode werd verruimd.",
                    original,
                    EventFactory.Lidmaatschap(verruimd)
                );

                continue;
            }

            var added = lidmaatschappen.VoegToe(
                new ToeTeVoegenLidmaatschap(
                    lidmaatschap.AndereVereniging,
                    lidmaatschap.AndereVerenigingNaam,
                    lidmaatschap.Geldigheidsperiode,
                    lidmaatschap.Identificatie,
                    lidmaatschap.Beschrijving
                )
            );
            lidmaatschappen = lidmaatschappen.Hydrate(lidmaatschappen.Append(added));

            resultaat.Toegevoegd(description, original, EventFactory.Lidmaatschap(added));
        }

        return resultaat.Build();
    }

    private static string? RedenNietOvergenomen(
        VerenigingState authentiekeVereniging,
        Lidmaatschap lidmaatschap,
        IReadOnlyDictionary<VCode, AndereVerenigingStatus> statusAndereVerenigingen
    )
    {
        var status = statusAndereVerenigingen.GetValueOrDefault(
            lidmaatschap.AndereVereniging,
            AndereVerenigingStatus.Actief
        );

        switch (status)
        {
            case AndereVerenigingStatus.Verwijderd:
                return RedenVerwijderdeVereniging;
            case AndereVerenigingStatus.Dubbel:
                return RedenDubbeleVereniging;
            case AndereVerenigingStatus.Gestopt:
                return RedenGestopteVereniging;
        }

        if (lidmaatschap.AndereVereniging == authentiekeVereniging.VCode)
            return RedenEigenVereniging;

        if (authentiekeVereniging.Verenigingssubtype.IsSubverenigingVan(lidmaatschap.AndereVereniging))
            return RedenSubvereniging;

        return null;
    }

    /// <summary>De vroegste startdatum en de laatste einddatum; een open start of einde blijft open.</summary>
    private static Geldigheidsperiode Verruim(Geldigheidsperiode bestaand, Geldigheidsperiode other) =>
        new(bestaand.Van <= other.Van ? bestaand.Van : other.Van, bestaand.Tot >= other.Tot ? bestaand.Tot : other.Tot);

    private static bool HeeftDezelfdeGeldigheidsperiode(Lidmaatschap lidmaatschap, Lidmaatschap other) =>
        lidmaatschap.Geldigheidsperiode.Van == other.Geldigheidsperiode.Van
        && lidmaatschap.Geldigheidsperiode.Tot == other.Geldigheidsperiode.Tot;

    private static string Describe(Lidmaatschap lidmaatschap) =>
        string.IsNullOrWhiteSpace(lidmaatschap.AndereVerenigingNaam)
            ? $"Lidmaatschap bij {lidmaatschap.AndereVereniging}"
            : $"Lidmaatschap bij '{lidmaatschap.AndereVerenigingNaam}' ({lidmaatschap.AndereVereniging})";
}
