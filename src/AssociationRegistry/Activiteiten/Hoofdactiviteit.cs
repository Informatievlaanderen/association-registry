namespace AssociationRegistry.Activiteiten;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Exceptions;

public class Hoofdactiviteit
{
    private static readonly List<Hoofdactiviteit> Hoofdactiviteiten = new()
    {
        new Hoofdactiviteit("BIAG", "Burgerinitiatief & Actiegroep"),
        new Hoofdactiviteit("BWWC", "Buurtwerking & Wijkcomité"),
        new Hoofdactiviteit("DINT", "Diversiteit & Integratie"),
        new Hoofdactiviteit("GEWE", "Gezondheid & Welzijn"),
        new Hoofdactiviteit("HOSP", "Hobby & Spel"),
        new Hoofdactiviteit("INOS", "Internationaal & Onwikkelingssamenwerking"),
        new Hoofdactiviteit("JGDW", "Jeugdwerk"),
        new Hoofdactiviteit("KLDU", "Klimaat & Duurzaamheid"),
        new Hoofdactiviteit("KECU", "Kunsten, Erfgoed & Cultuur"),
        new Hoofdactiviteit("LAVI", "Landbouw & Visserij"),
        new Hoofdactiviteit("LEBE", "Levenbeschouwelijk"),
        new Hoofdactiviteit("MADP", "Maatschappelijke dienstverlening & Politiek"),
        new Hoofdactiviteit("MEDG", "Mediawijsheid & Digitale geletterdheid"),
        new Hoofdactiviteit("MROW", "Mobiliteit, Ruimtelijke ordening & Wonen"),
        new Hoofdactiviteit("NMDW", "Natuur, Milieu & Dierenwelzijn"),
        new Hoofdactiviteit("VORM", "Onderwijs & Vorming"),
        new Hoofdactiviteit("SOVO", "Sociaal-cultureel volwassenenwerk"),
        new Hoofdactiviteit("SPRT", "Sport"),
        new Hoofdactiviteit("ONWE", "Technologie & Wetenschap"),
        new Hoofdactiviteit("TOER", "Toerisme"),
        new Hoofdactiviteit("WESE", "Werk & Sociale economie"),
    };

    public string Beschrijving { get; }
    public string Code { get; }

    private Hoofdactiviteit(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public static Hoofdactiviteit Create(string key)
    {
        var value = Hoofdactiviteiten.SingleOrDefault(p => p.Code == key);
        return value ?? throw new UnknownHoofdactiviteitCode();
    }

    public static IImmutableList<Hoofdactiviteit> All()
        => Hoofdactiviteiten.ToImmutableList();
}
