namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using System.Collections.ObjectModel;

public readonly record struct VertegenwoordigersKboDiff(
    Vertegenwoordiger[] Toegevoegd,
    Vertegenwoordiger[] Gewijzigd,
    Vertegenwoordiger[] Verwijderd
)
{
    public static VertegenwoordigersKboDiff BerekenVerschillen(Vertegenwoordigers huidigeVertegenwoordigers, IEnumerable<Vertegenwoordiger> vertegenwoordigersVolgensKbo)
    {
        var vertegenwoordigersUitKbo = vertegenwoordigersVolgensKbo.ToArray();
        var inszNrsUitKbo = new HashSet<string>(vertegenwoordigersUitKbo.Select(x => x.Insz.ToString()));
        var inszNrsInVereniging = huidigeVertegenwoordigers.ToDictionary(v => v.Insz, v => v);

        var gewijzigdeVertegenwoordigers = FindGewijzigdeVertegenwoordigers(huidigeVertegenwoordigers, vertegenwoordigersUitKbo, inszNrsInVereniging);
        var verwijderdeVertegenwoordigers = FindVerwijderdeVertegenwoordigers(huidigeVertegenwoordigers, inszNrsUitKbo);
        var toegevoegdeVertegenwoordigers = FindToegevoegdeVertegenwoordigers(huidigeVertegenwoordigers.NextId, vertegenwoordigersUitKbo, inszNrsInVereniging);

        return new VertegenwoordigersKboDiff(
            Toegevoegd: toegevoegdeVertegenwoordigers.ToArray(),
            Gewijzigd : gewijzigdeVertegenwoordigers.ToArray(),
            Verwijderd: verwijderdeVertegenwoordigers.ToArray()
        );
    }

    private static IReadOnlyCollection<Vertegenwoordiger> FindGewijzigdeVertegenwoordigers(
        Vertegenwoordigers huidigeVertegenwoordigers,
        IEnumerable<Vertegenwoordiger> vertegenwoordigersUitKbo,
        Dictionary<Insz, Vertegenwoordiger> inszNrsInVereniging)
    {
        var huidigeVertegenwoordigerIdsPerInsz = huidigeVertegenwoordigers.ToDictionary(v => v.Insz, v => v.VertegenwoordigerId);

        var teWijzigen = vertegenwoordigersUitKbo
                        .Where(kbo => huidigeVertegenwoordigerIdsPerInsz.ContainsKey(kbo.Insz))
                        .Select(kbo => kbo with { VertegenwoordigerId = huidigeVertegenwoordigerIdsPerInsz[kbo.Insz] })
                        .ToList();

        var gewijzigdeVertegenwoordigers = teWijzigen
                                          .Where(nieuw => !nieuw.WouldBeEquivalent(inszNrsInVereniging[nieuw.Insz]))
                                          .ToArray();

        return new ReadOnlyCollection<Vertegenwoordiger>(gewijzigdeVertegenwoordigers);
    }

    private static IEnumerable<Vertegenwoordiger> FindVerwijderdeVertegenwoordigers(
        Vertegenwoordigers huidigeVertegenwoordigers,
        HashSet<string> inszNrsUitKbo)
    {
        return huidigeVertegenwoordigers.Where(s => !inszNrsUitKbo.Contains(s.Insz));
    }

    private static List<Vertegenwoordiger> FindToegevoegdeVertegenwoordigers(
        int nextId,
        Vertegenwoordiger[] vertegenwoordigersUitKbo,
        Dictionary<Insz, Vertegenwoordiger> inszNrsInVereniging)
    {
        var toegevoegdeVertegenwoordigers = new List<Vertegenwoordiger>();
        foreach (var v in vertegenwoordigersUitKbo)
        {
            if (!inszNrsInVereniging.ContainsKey(v.Insz))
                toegevoegdeVertegenwoordigers.Add(v with
                {
                    VertegenwoordigerId = nextId++,
                });
        }

        return toegevoegdeVertegenwoordigers;
    }
}
