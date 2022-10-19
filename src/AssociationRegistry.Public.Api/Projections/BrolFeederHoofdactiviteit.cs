namespace AssociationRegistry.Public.Api.Projections;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Copy from Admin, to make brolfeeder work
/// </summary>
public class BrolFeederHoofdactiviteit
{
    private static readonly List<BrolFeederHoofdactiviteit> Hoofdactiviteiten = new()
    {
        new BrolFeederHoofdactiviteit("Buurtwerking"),
        new BrolFeederHoofdactiviteit("Burgerinitiatief & Actiegroep"),
        new BrolFeederHoofdactiviteit("Cultuur"),
        new BrolFeederHoofdactiviteit("Creatief / Hobby"),
        new BrolFeederHoofdactiviteit("Diversiteit & Integratie"),
        new BrolFeederHoofdactiviteit("Duurzaamheid"),
        new BrolFeederHoofdactiviteit("Gezondheid & Welzijn"),
        new BrolFeederHoofdactiviteit("Internationaal & Onwikkelingssamenwerking"),
        new BrolFeederHoofdactiviteit("Jeugdwerk"),
        new BrolFeederHoofdactiviteit("Kunst, Erfgoed, Sociaal-cultureel werk & Media"),
        new BrolFeederHoofdactiviteit("Levenbeschouwelijk"),
        new BrolFeederHoofdactiviteit("Maatschappelijke dienstverlening"),
        new BrolFeederHoofdactiviteit("Natuur & Klimaat & Milieu & Dierenwelzijn"),
        new BrolFeederHoofdactiviteit("Onderwijs & Wetenschap"),
        new BrolFeederHoofdactiviteit("Sport"),
        new BrolFeederHoofdactiviteit("Toerisme"),
        new BrolFeederHoofdactiviteit("Vorming"),
    };

    private readonly string _naam;

    private BrolFeederHoofdactiviteit(string naam)
        => _naam = naam;

    public static BrolFeederHoofdactiviteit Create(string key)
    {
        var value = Hoofdactiviteiten.SingleOrDefault(p => p._naam == key);
        return value ?? throw new KeyNotFoundException(key);
    }

    public static IImmutableList<BrolFeederHoofdactiviteit> All()
        => Hoofdactiviteiten.ToImmutableList();

    public override string ToString()
        => _naam;
}
