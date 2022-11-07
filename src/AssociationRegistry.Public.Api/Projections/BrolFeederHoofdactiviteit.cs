namespace AssociationRegistry.Public.Api.Projections;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Copy from Admin, to make brolfeeder work
/// </summary>
public class BrolFeederHoofdactiviteit
{
    private static readonly List<BrolFeederHoofdactiviteit> BrolFeederHoofdactiviteiten = new()
    {
        new BrolFeederHoofdactiviteit("BWRK", "Buurtwerking"),
        new BrolFeederHoofdactiviteit("BIAG", "Burgerinitiatief & Actiegroep"),
        new BrolFeederHoofdactiviteit("CULT", "Cultuur"),
        new BrolFeederHoofdactiviteit("CREA", "Creatief / Hobby"),
        new BrolFeederHoofdactiviteit("DINT", "Diversiteit & Integratie"),
        new BrolFeederHoofdactiviteit("DRZH", "Duurzaamheid"),
        new BrolFeederHoofdactiviteit("GEWE", "Gezondheid & Welzijn"),
        new BrolFeederHoofdactiviteit("INOS", "Internationaal & Onwikkelingssamenwerking"),
        new BrolFeederHoofdactiviteit("JGDW", "Jeugdwerk"),
        new BrolFeederHoofdactiviteit("KESM", "Kunst, Erfgoed, Sociaal-cultureel werk & Media"),
        new BrolFeederHoofdactiviteit("LEBE", "Levenbeschouwelijk"),
        new BrolFeederHoofdactiviteit("MADI", "Maatschappelijke dienstverlening"),
        new BrolFeederHoofdactiviteit("NKMD", "Natuur & Klimaat & Milieu & Dierenwelzijn"),
        new BrolFeederHoofdactiviteit("ONWE", "Onderwijs & Wetenschap"),
        new BrolFeederHoofdactiviteit("SPRT", "Sport"),
        new BrolFeederHoofdactiviteit("TOER", "Toerisme"),
        new BrolFeederHoofdactiviteit("VORM", "Vorming"),
    };

    public string Naam { get; }
    public string Code { get; }

    private BrolFeederHoofdactiviteit(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static BrolFeederHoofdactiviteit Create(string key)
    {
        var value = BrolFeederHoofdactiviteiten.SingleOrDefault(p => p.Code == key);
        return value ?? throw new KeyNotFoundException(key);
    }

    public static IImmutableList<BrolFeederHoofdactiviteit> All()
        => BrolFeederHoofdactiviteiten.ToImmutableList();
}
