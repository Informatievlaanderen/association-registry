namespace AssociationRegistry.Admin.Api.Verenigingen.Activiteiten;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class Hoofdactiviteit
{
    private static readonly List<Hoofdactiviteit> Hoofdactiviteiten = new()
    {
        new Hoofdactiviteit("BWRK", "Buurtwerking"),
        new Hoofdactiviteit("BIAG", "Burgerinitiatief & Actiegroep"),
        new Hoofdactiviteit("CULT", "Cultuur"),
        new Hoofdactiviteit("CREA", "Creatief / Hobby"),
        new Hoofdactiviteit("DINT", "Diversiteit & Integratie"),
        new Hoofdactiviteit("DRZH", "Duurzaamheid"),
        new Hoofdactiviteit("GEWE", "Gezondheid & Welzijn"),
        new Hoofdactiviteit("INOS", "Internationaal & Onwikkelingssamenwerking"),
        new Hoofdactiviteit("JGDW", "Jeugdwerk"),
        new Hoofdactiviteit("KESM", "Kunst, Erfgoed, Sociaal-cultureel werk & Media"),
        new Hoofdactiviteit("LEBE", "Levenbeschouwelijk"),
        new Hoofdactiviteit("MADI", "Maatschappelijke dienstverlening"),
        new Hoofdactiviteit("NKMD", "Natuur & Klimaat & Milieu & Dierenwelzijn"),
        new Hoofdactiviteit("ONWE", "Onderwijs & Wetenschap"),
        new Hoofdactiviteit("SPRT", "Sport"),
        new Hoofdactiviteit("TOER", "Toerisme"),
        new Hoofdactiviteit("VORM", "Vorming"),
    };

    public string Naam { get; }
    public string Code { get; }

    private Hoofdactiviteit(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static Hoofdactiviteit Create(string key)
    {
        var value = Hoofdactiviteiten.SingleOrDefault(p => p.Code == key);
        return value ?? throw new KeyNotFoundException(key);
    }

    public static IImmutableList<Hoofdactiviteit> All()
        => Hoofdactiviteiten.ToImmutableList();
}
