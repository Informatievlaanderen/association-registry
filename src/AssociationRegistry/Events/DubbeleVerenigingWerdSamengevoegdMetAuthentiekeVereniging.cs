namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using DecentraalBeheer.Vereniging.Erkenningen;
using Vereniging.Bronnen;

/// <summary>
///     Data uit de dubbele vereniging werd samengevoegd met de authentieke vereniging, na de automatische merge.
///     Staat op de stream van de authentieke vereniging (<see cref="VCode" />).
///     Dit is een andere actie dan het markeren als dubbel (<see cref="VerenigingWerdGemarkeerdAlsDubbelVan" />).
///     Het tijdstip en de initiator van de samenvoeging zitten, zoals bij elk event, in de metadata.
///     De persoonsgegevens van vertegenwoordigers en bankrekeningnummers worden apart bewaard onder de authentieke
///     vereniging, zodat ze niet verdwijnen wanneer de bewaartermijn van de dubbele vereniging verloopt.
/// </summary>
public record DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVerenigingZonderPersoonsgegevens(
    string VCode,
    string VCodeDubbeleVereniging,
    SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.VertegenwoordigerZonderPersoonsgegevens,
        Registratiedata.BankrekeningnummerZonderPersoonsgegevens
    > OvergenomenGegevens,
    SamenvoegingDubbeleVereniging.NietOvergenomenGegevensPerGroep<
        Registratiedata.VertegenwoordigerZonderPersoonsgegevens,
        Registratiedata.BankrekeningnummerZonderPersoonsgegevens
    > NietOvergenomenGegevens
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;
}

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    string VCode,
    string VCodeDubbeleVereniging,
    SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        Registratiedata.Bankrekeningnummer
    > OvergenomenGegevens,
    SamenvoegingDubbeleVereniging.NietOvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        Registratiedata.Bankrekeningnummer
    > NietOvergenomenGegevens
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"VCodeDubbeleVereniging = {VCodeDubbeleVereniging}, ");
        builder.Append($"OvergenomenGegevens = {OvergenomenGegevens.Aantal} items, ");
        builder.Append($"NietOvergenomenGegevens = {NietOvergenomenGegevens.Aantal} items");

        return true;
    }
}

/// <summary>
///     De inhoud van <see cref="DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging" />.
///     De gegevens zijn gegroepeerd per historiekcategorie; de groep van een gegeven volgt uit waar het staat.
///     Vertegenwoordigers en bankrekeningnummers zijn generiek, zodat het bewaarde event enkel een verwijzing naar de
///     persoonsgegevens bevat en het geüpcaste event de persoonsgegevens zelf.
/// </summary>
public static class SamenvoegingDubbeleVereniging
{
    public static class Acties
    {
        /// <summary>Een nieuw item werd toegevoegd aan de authentieke vereniging.</summary>
        public const string Toegevoegd = "Toegevoegd";

        /// <summary>Een leeg attribuut op de authentieke vereniging werd ingevuld.</summary>
        public const string Verrijkt = "Verrijkt";

        /// <summary>Een item werd toegevoegd met aangepaste metadata, bv. isPrimair=true.</summary>
        public const string ToegevoegdMetAanpassing = "ToegevoegdMetAanpassing";

        /// <summary>Het item bestond al op de authentieke vereniging en werd niet dubbel toegevoegd.</summary>
        public const string Gededupliceerd = "Gededupliceerd";

        /// <summary>Het item werd niet overgenomen, zie de reden.</summary>
        public const string NietOvergenomen = "NietOvergenomen";
    }

    public static class Velden
    {
        public const string KorteNaam = "korteNaam";
        public const string KorteBeschrijving = "korteBeschrijving";
        public const string Startdatum = "startdatum";
        public const string Doelgroep = "doelgroep";
    }

    /// <summary>
    ///     Een gegeven dat op de authentieke vereniging terechtkwam.
    ///     <see cref="OrigineleWaarde" /> is de waarde op de dubbele vereniging, <see cref="NieuweWaarde" /> de waarde
    ///     op de authentieke vereniging. Ze verschillen enkel bij <see cref="Acties.ToegevoegdMetAanpassing" />.
    /// </summary>
    /// <param name="Actie">Toegevoegd, Verrijkt of ToegevoegdMetAanpassing.</param>
    /// <param name="Beschrijving">Leesbare beschrijving voor de historiek.</param>
    public record OvergenomenGegeven<T>(string Actie, string Beschrijving, T OrigineleWaarde, T NieuweWaarde);

    /// <summary>Een gegeven van de dubbele vereniging dat niet op de authentieke vereniging terechtkwam.</summary>
    /// <param name="Actie">Gededupliceerd of NietOvergenomen.</param>
    /// <param name="Reden">Waarom het gegeven niet werd overgenomen.</param>
    /// <param name="Beschrijving">Leesbare beschrijving voor de historiek.</param>
    /// <param name="Waarde">De waarde op de dubbele vereniging.</param>
    public record NietOvergenomenGegeven<T>(string Actie, string Reden, string Beschrijving, T Waarde);

    /// <summary>Een attribuut uit de basisgegevens, zie <see cref="Velden" />.</summary>
    public record Basisgegeven(string Veld, string Waarde);

    public record Erkenning(
        int ErkenningId,
        IpdcProduct IpdcProduct,
        DateOnly? Startdatum,
        DateOnly? Einddatum,
        DateOnly? Hernieuwingsdatum,
        string HernieuwingsUrl,
        string Status
    );

    public record OvergenomenGegevensPerGroep<TVertegenwoordiger, TBankrekeningnummer>(
        OvergenomenGegeven<Basisgegeven>[] Basisgegevens,
        OvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>[] Hoofdactiviteiten,
        OvergenomenGegeven<Registratiedata.Werkingsgebied>[] Werkingsgebieden,
        OvergenomenGegeven<Registratiedata.Contactgegeven>[] Contactgegevens,
        OvergenomenGegeven<Registratiedata.Locatie>[] Locaties,
        OvergenomenGegeven<TVertegenwoordiger>[] Vertegenwoordigers,
        OvergenomenGegeven<TBankrekeningnummer>[] Bankrekeningnummers,
        OvergenomenGegeven<Erkenning>[] Erkenningen,
        OvergenomenGegeven<Registratiedata.SubverenigingVan>[] Relaties,
        OvergenomenGegeven<Registratiedata.Lidmaatschap>[] Lidmaatschappen
    )
    {
        [IgnoreDataMember]
        public int Aantal =>
            Basisgegevens.Length
            + Hoofdactiviteiten.Length
            + Werkingsgebieden.Length
            + Contactgegevens.Length
            + Locaties.Length
            + Vertegenwoordigers.Length
            + Bankrekeningnummers.Length
            + Erkenningen.Length
            + Relaties.Length
            + Lidmaatschappen.Length;

        /// <summary>Dezelfde gegevens, met andere vertegenwoordigers en bankrekeningnummers.</summary>
        public OvergenomenGegevensPerGroep<TOtherVertegenwoordiger, TOtherBankrekeningnummer> With<
            TOtherVertegenwoordiger,
            TOtherBankrekeningnummer
        >(
            OvergenomenGegeven<TOtherVertegenwoordiger>[] vertegenwoordigers,
            OvergenomenGegeven<TOtherBankrekeningnummer>[] bankrekeningnummers
        ) =>
            new(
                Basisgegevens,
                Hoofdactiviteiten,
                Werkingsgebieden,
                Contactgegevens,
                Locaties,
                vertegenwoordigers,
                bankrekeningnummers,
                Erkenningen,
                Relaties,
                Lidmaatschappen
            );
    }

    public record NietOvergenomenGegevensPerGroep<TVertegenwoordiger, TBankrekeningnummer>(
        NietOvergenomenGegeven<Basisgegeven>[] Basisgegevens,
        NietOvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>[] Hoofdactiviteiten,
        NietOvergenomenGegeven<Registratiedata.Werkingsgebied>[] Werkingsgebieden,
        NietOvergenomenGegeven<Registratiedata.Contactgegeven>[] Contactgegevens,
        NietOvergenomenGegeven<Registratiedata.Locatie>[] Locaties,
        NietOvergenomenGegeven<TVertegenwoordiger>[] Vertegenwoordigers,
        NietOvergenomenGegeven<TBankrekeningnummer>[] Bankrekeningnummers,
        NietOvergenomenGegeven<Erkenning>[] Erkenningen,
        NietOvergenomenGegeven<Registratiedata.SubverenigingVan>[] Relaties,
        NietOvergenomenGegeven<Registratiedata.Lidmaatschap>[] Lidmaatschappen
    )
    {
        [IgnoreDataMember]
        public int Aantal =>
            Basisgegevens.Length
            + Hoofdactiviteiten.Length
            + Werkingsgebieden.Length
            + Contactgegevens.Length
            + Locaties.Length
            + Vertegenwoordigers.Length
            + Bankrekeningnummers.Length
            + Erkenningen.Length
            + Relaties.Length
            + Lidmaatschappen.Length;

        /// <summary>Dezelfde gegevens, met andere vertegenwoordigers en bankrekeningnummers.</summary>
        public NietOvergenomenGegevensPerGroep<TOtherVertegenwoordiger, TOtherBankrekeningnummer> With<
            TOtherVertegenwoordiger,
            TOtherBankrekeningnummer
        >(
            NietOvergenomenGegeven<TOtherVertegenwoordiger>[] vertegenwoordigers,
            NietOvergenomenGegeven<TOtherBankrekeningnummer>[] bankrekeningnummers
        ) =>
            new(
                Basisgegevens,
                Hoofdactiviteiten,
                Werkingsgebieden,
                Contactgegevens,
                Locaties,
                vertegenwoordigers,
                bankrekeningnummers,
                Erkenningen,
                Relaties,
                Lidmaatschappen
            );
    }
}
