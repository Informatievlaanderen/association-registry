namespace AssociationRegistry.Public.Api.Projections;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Copy from Admin, to make brolfeeder work
/// </summary>
public class Protput
{
    private static readonly List<Protput> Protputten = new()
    {
        new Protput("Buurtwerking"),
        new Protput("Burgerinitiatief & Actiegroep"),
        new Protput("Cultuur"),
        new Protput("Creatief / Hobby"),
        new Protput("Diversiteit & Integratie"),
        new Protput("Duurzaamheid"),
        new Protput("Gezondheid & Welzijn"),
        new Protput("Internationaal & Onwikkelingssamenwerking"),
        new Protput("Jeugdwerk"),
        new Protput("Kunst, Erfgoed, Sociaal-cultureel werk & Media"),
        new Protput("Levenbeschouwelijk"),
        new Protput("Maatschappelijke dienstverlening"),
        new Protput("Natuur & Klimaat & Milieu & Dierenwelzijn"),
        new Protput("Onderwijs & Wetenschap"),
        new Protput("Sport"),
        new Protput("Toerisme"),
        new Protput("Vorming"),
    };

    private readonly string _naam;

    private Protput(string naam)
        => _naam = naam;

    public static Protput Create(string key)
    {
        var value = Protputten.SingleOrDefault(p => p._naam == key);
        return value ?? throw new KeyNotFoundException(key);
    }

    public static IImmutableList<Protput> All()
        => Protputten.ToImmutableList();

    public override string ToString()
        => _naam;
}
