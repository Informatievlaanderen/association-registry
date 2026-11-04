namespace AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging;

using Gegevens = Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Verzamelt per gegeven van de dubbele vereniging wat ermee gebeurde, met de beschrijvingen voor de historiek.
/// </summary>
internal sealed class SamenvoegingResultaat<T>
{
    private readonly List<Gegevens.OvergenomenGegeven<T>> _overgenomen = [];
    private readonly List<Gegevens.NietOvergenomenGegeven<T>> _nietOvergenomen = [];

    public void Toegevoegd(string omschrijving, T origineleWaarde, T nieuweWaarde) =>
        _overgenomen.Add(
            new Gegevens.OvergenomenGegeven<T>(
                Gegevens.Acties.Toegevoegd,
                $"{omschrijving} werd toegevoegd vanuit de dubbele vereniging.",
                origineleWaarde,
                nieuweWaarde
            )
        );

    /// <param name="authentiekeHeeftAl">Wat de authentieke vereniging al heeft, bv. "een primaire locatie".</param>
    public void ToegevoegdAlsNietPrimair(
        string omschrijving,
        string authentiekeHeeftAl,
        T origineleWaarde,
        T nieuweWaarde
    ) =>
        _overgenomen.Add(
            Gegevens.ToegevoegdMetAanpassing(
                $"{omschrijving} werd toegevoegd als niet-primair, omdat de authentieke vereniging al {authentiekeHeeftAl} heeft.",
                origineleWaarde,
                nieuweWaarde
            )
        );

    public void Gededupliceerd(string omschrijving, T waarde) =>
        _nietOvergenomen.Add(Gegevens.Gededupliceerd(omschrijving, waarde));

    public void NietOvergenomen(string omschrijving, T waarde, string reden) =>
        _nietOvergenomen.Add(Gegevens.NietOvergenomen(omschrijving, waarde, reden));

    public (
        Gegevens.OvergenomenGegeven<T>[] Overgenomen,
        Gegevens.NietOvergenomenGegeven<T>[] NietOvergenomen
    ) Build() => ([.. _overgenomen], [.. _nietOvergenomen]);
}
