namespace AssociationRegistry.Admin.Api.Verenigingen.Activiteiten;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class Hoofdactiviteit
{
    private static readonly List<Hoofdactiviteit> Hoofdactiviteiten = new()
    {
        new Hoofdactiviteit("Buurtwerking"),
        new Hoofdactiviteit("Burgerinitiatief & Actiegroep"),
        new Hoofdactiviteit("Cultuur"),
        new Hoofdactiviteit("Creatief / Hobby"),
        new Hoofdactiviteit("Diversiteit & Integratie"),
        new Hoofdactiviteit("Duurzaamheid"),
        new Hoofdactiviteit("Gezondheid & Welzijn"),
        new Hoofdactiviteit("Internationaal & Onwikkelingssamenwerking"),
        new Hoofdactiviteit("Jeugdwerk"),
        new Hoofdactiviteit("Kunst, Erfgoed, Sociaal-cultureel werk & Media"),
        new Hoofdactiviteit("Levenbeschouwelijk"),
        new Hoofdactiviteit("Maatschappelijke dienstverlening"),
        new Hoofdactiviteit("Natuur & Klimaat & Milieu & Dierenwelzijn"),
        new Hoofdactiviteit("Onderwijs & Wetenschap"),
        new Hoofdactiviteit("Sport"),
        new Hoofdactiviteit("Toerisme"),
        new Hoofdactiviteit("Vorming"),
    };

    private readonly string _naam;

    private Hoofdactiviteit(string naam)
        => _naam = naam;

    public static Hoofdactiviteit Create(string key)
    {
        var value = Hoofdactiviteiten.SingleOrDefault(p => p._naam == key);
        return value ?? throw new KeyNotFoundException(key);
    }

    public static IImmutableList<Hoofdactiviteit> All()
        => Hoofdactiviteiten.ToImmutableList();

    public override string ToString()
        => _naam;
}
