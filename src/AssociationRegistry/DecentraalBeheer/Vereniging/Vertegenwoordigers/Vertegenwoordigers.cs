namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;
using Framework;
using Emails;
using Exceptions;
using SocialMedias;
using System.Collections.ObjectModel;
using TelefoonNummers;

public class Vertegenwoordigers : ReadOnlyCollection<Vertegenwoordiger>
{
    private const int InitialId = 1;
    public int NextId { get; }

    private Vertegenwoordiger? Primair
        => Items.SingleOrDefault(i => i.IsPrimair);

    private Vertegenwoordigers(IEnumerable<Vertegenwoordiger> vertegenwoordigers, int nextId) : base(vertegenwoordigers.ToArray())
    {
        NextId = nextId;
    }

    public static Vertegenwoordigers Empty
        => new(Array.Empty<Vertegenwoordiger>(), InitialId);

    public Vertegenwoordigers Hydrate(IEnumerable<Vertegenwoordiger> vertegenwoordigers)
    {
        vertegenwoordigers = vertegenwoordigers.ToArray();

        if (!vertegenwoordigers.Any())
            return new Vertegenwoordigers(Empty, Math.Max(InitialId, NextId));

        return new Vertegenwoordigers(vertegenwoordigers, Math.Max(vertegenwoordigers.Max(x => x.VertegenwoordigerId) + 1, NextId));
    }

    public Vertegenwoordiger[] VoegToe(params Vertegenwoordiger[] toeTeVoegenVertegenwoordigers)
    {
        var vertegenwoordigers = this;
        var toegevoegdeVertegenwoordigers = Array.Empty<Vertegenwoordiger>();

        foreach (var toeTeVoegenVertegenwoordiger in toeTeVoegenVertegenwoordigers)
        {
            var vertegenwoordigerMetId = vertegenwoordigers.VoegToe(toeTeVoegenVertegenwoordiger);

            vertegenwoordigers = new Vertegenwoordigers(vertegenwoordigers.Append(vertegenwoordigerMetId), vertegenwoordigers.NextId + 1);

            toegevoegdeVertegenwoordigers = toegevoegdeVertegenwoordigers.Append(vertegenwoordigerMetId).ToArray();
        }

        return toegevoegdeVertegenwoordigers;
    }

    public Vertegenwoordiger VoegToe(Vertegenwoordiger toeTeVoegenVertegenwoordiger)
    {
        ThrowIfCannotAppend(toeTeVoegenVertegenwoordiger);

        return toeTeVoegenVertegenwoordiger with { VertegenwoordigerId = NextId };
    }

    public Vertegenwoordiger? Wijzig(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair)
    {
        MustContain(vertegenwoordigerId);

        if (this[vertegenwoordigerId].WouldBeEquivalent(rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair,
                                                        out var gewijzigdeVertegenwoordiger))
            return null;

        MustNotHaveMultiplePrimary(gewijzigdeVertegenwoordiger);

        return gewijzigdeVertegenwoordiger;
    }

    public Vertegenwoordiger Verwijder(int vertegenwoordigerId)
    {
        MustContain(vertegenwoordigerId);
        MustHaveMoreThanOneVertegenwoordiger();

        return this[vertegenwoordigerId];
    }

    private void MustHaveMoreThanOneVertegenwoordiger()
    {
        Throw<LaatsteVertegenwoordigerKanNietVerwijderdWorden>.If(Items.Count == 1);
    }

    private void ThrowIfCannotAppend(Vertegenwoordiger vertegenwoordiger)
    {
        var vertegenwoordigers = this.Append(vertegenwoordiger).ToArray();

        Throw<InszMoetUniekZijn>.If(HasDuplicateInsz(vertegenwoordigers));
        Throw<MeerderePrimaireVertegenwoordigers>.If(HasMultiplePrimaryVertegenwoordigers(vertegenwoordigers));
    }

    public new Vertegenwoordiger this[int vertegenwoordigerId]
        => this.Single(v => v.VertegenwoordigerId == vertegenwoordigerId);

    private static bool HasMultiplePrimaryVertegenwoordigers(Vertegenwoordiger[] vertegenwoordigersArray)
        => vertegenwoordigersArray.Count(vertegenwoordiger => vertegenwoordiger.IsPrimair) > 1;

    private static bool HasDuplicateInsz(Vertegenwoordiger[] vertegenwoordigers)
        => vertegenwoordigers.DistinctBy(x => x.Insz).Count() != vertegenwoordigers.Length;

    public void MustNotHaveDuplicateOf(Vertegenwoordiger vertegenwoordiger)
        => Throw<InszMoetUniekZijn>.If(
            HasDuplicateInsz(Items.Append(vertegenwoordiger).ToArray()));

    private void MustNotHaveMultiplePrimary(Vertegenwoordiger vertegenwoordiger)
    {
        Throw<MeerderePrimaireVertegenwoordigers>.If(
            Primair is not null && // there is a primair vertegenwoordiger
            Primair.VertegenwoordigerId != vertegenwoordiger.VertegenwoordigerId && // it is not the same
            vertegenwoordiger.IsPrimair); // we want to make it primair
    }

    private void MustContain(int vertegenwoordigerId)
    {
        Throw<VertegenwoordigerIsNietGekend>.IfNot(
            Items.Any(vertegenwoordiger => vertegenwoordiger.VertegenwoordigerId == vertegenwoordigerId),
            vertegenwoordigerId.ToString());
    }

    public IReadOnlyCollection<Vertegenwoordiger> ProvideTeWijzigenVertegenwoordigers(IEnumerable<Vertegenwoordiger> vertegenwoordigersCreatedFromKbo)
    {
        var existingInszToId = this.ToDictionary(v => v.Insz, v => v.VertegenwoordigerId);

        var teWijzigen = vertegenwoordigersCreatedFromKbo
            .Where(kbo => existingInszToId.ContainsKey(kbo.Insz))
            .Select(kbo => kbo with { VertegenwoordigerId = existingInszToId[kbo.Insz] })
            .ToList();

        return new ReadOnlyCollection<Vertegenwoordiger>(teWijzigen);
    }

    public VertegenwoordigersKboDiff SyncMetKboVertegenwoordigers(IEnumerable<Vertegenwoordiger> vertegenwoordigersVolgensKbo)
    {
        var inkomend = vertegenwoordigersVolgensKbo.ToArray();

        var existingByInsz = this.ToDictionary(v => v.Insz, v => v);

        var toeTeVoegen = new List<Vertegenwoordiger>();
        var nextId = NextId;

        foreach (var v in inkomend)
        {
            if (existingByInsz.ContainsKey(v.Insz))
                continue;

            toeTeVoegen.Add(v with { VertegenwoordigerId = nextId++ });
        }

        var teWijzigenMetId = ProvideTeWijzigenVertegenwoordigers(inkomend);
        var gewijzigd = teWijzigenMetId
            .Where(nieuw =>
            {
                var oud = existingByInsz[nieuw.Insz];
                return !nieuw.WouldBeEquivalent(oud);
            })
            .ToArray();

        var inkomendInsz = new HashSet<string>(inkomend.Select(x => x.Insz.ToString()));
        var verwijderd = this.Where(s => !inkomendInsz.Contains(s.Insz)).ToArray();

        return new VertegenwoordigersKboDiff(
            Toegevoegd: toeTeVoegen.ToArray(),
            Gewijzigd : gewijzigd,
            Verwijderd: verwijderd
        );
    }
}

public readonly record struct VertegenwoordigersKboDiff(
    Vertegenwoordiger[] Toegevoegd,
    Vertegenwoordiger[] Gewijzigd,
    Vertegenwoordiger[] Verwijderd
);

public static class VertegenwoordigerEnumerableExtensions
{
    public static IEnumerable<Vertegenwoordiger> Without(this IEnumerable<Vertegenwoordiger> source, Vertegenwoordiger vertegenwoordiger)
    {
        return source.Where(c => c.VertegenwoordigerId != vertegenwoordiger.VertegenwoordigerId);
    }

    public static IEnumerable<Vertegenwoordiger> Without(this IEnumerable<Vertegenwoordiger> source, int vertegenwoordigerId)
    {
        return source.Where(c => c.VertegenwoordigerId != vertegenwoordigerId);
    }

    public static IEnumerable<Vertegenwoordiger> AppendFromEventData(
        this IEnumerable<Vertegenwoordiger> vertegenwoordigers,
        VertegenwoordigerWerdOvergenomenUitKBO eventData)
        => vertegenwoordigers.Append(
            Vertegenwoordiger.Hydrate(
                eventData.VertegenwoordigerId,
                Insz.Hydrate(eventData.Insz),
                string.Empty,
                string.Empty,
                Voornaam.Hydrate(eventData.Voornaam),
                Achternaam.Hydrate(eventData.Achternaam),
                isPrimair: false,
                Email.Leeg,
                TelefoonNummer.Leeg,
                TelefoonNummer.Leeg,
                SocialMedia.Leeg));

    public static IEnumerable<Vertegenwoordiger> AppendFromEventData(
        this IEnumerable<Vertegenwoordiger> vertegenwoordigers,
        VertegenwoordigerWerdToegevoegdVanuitKBO eventData)
        => vertegenwoordigers.Append(
            Vertegenwoordiger.Hydrate(
                eventData.VertegenwoordigerId,
                Insz.Hydrate(eventData.Insz),
                string.Empty,
                string.Empty,
                Voornaam.Hydrate(eventData.Voornaam),
                Achternaam.Hydrate(eventData.Achternaam),
                isPrimair: false,
                Email.Leeg,
                TelefoonNummer.Leeg,
                TelefoonNummer.Leeg,
                SocialMedia.Leeg));

    public static IEnumerable<Vertegenwoordiger> AppendFromEventData(
        this IEnumerable<Vertegenwoordiger> vertegenwoordigers,
        VertegenwoordigerWerdGewijzigdInKBO eventData)
        => vertegenwoordigers.Append(
            Vertegenwoordiger.Hydrate(
                eventData.VertegenwoordigerId,
                Insz.Hydrate(eventData.Insz),
                string.Empty,
                string.Empty,
                Voornaam.Hydrate(eventData.Voornaam),
                Achternaam.Hydrate(eventData.Achternaam),
                isPrimair: false,
                Email.Leeg,
                TelefoonNummer.Leeg,
                TelefoonNummer.Leeg,
                SocialMedia.Leeg));
}
