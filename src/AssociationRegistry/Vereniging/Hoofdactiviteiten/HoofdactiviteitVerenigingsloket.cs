namespace AssociationRegistry.Vereniging;

using Exceptions;
using System.Collections.Immutable;

public record HoofdactiviteitVerenigingsloket
{
    private static readonly List<HoofdactiviteitVerenigingsloket> HoofdactiviteitenVerenigingsloket = new()
    {
        new HoofdactiviteitVerenigingsloket(code: "BIAG", naam: "Burgerinitiatief & Actiegroep"),
        new HoofdactiviteitVerenigingsloket(code: "BWWC", naam: "Buurtwerking & Wijkcomité"),
        new HoofdactiviteitVerenigingsloket(code: "DINT", naam: "Diversiteit & Integratie"),
        new HoofdactiviteitVerenigingsloket(code: "GEWE", naam: "Gezondheid & Welzijn"),
        new HoofdactiviteitVerenigingsloket(code: "HOSP", naam: "Hobby & Spel"),
        new HoofdactiviteitVerenigingsloket(code: "INOS", naam: "Internationaal & Ontwikkelingssamenwerking"),
        new HoofdactiviteitVerenigingsloket(code: "JGDW", naam: "Jeugdwerk"),
        new HoofdactiviteitVerenigingsloket(code: "KLDU", naam: "Klimaat & Duurzaamheid"),
        new HoofdactiviteitVerenigingsloket(code: "KECU", naam: "Kunsten, Erfgoed & Cultuur"),
        new HoofdactiviteitVerenigingsloket(code: "LAVI", naam: "Landbouw & Visserij"),
        new HoofdactiviteitVerenigingsloket(code: "LEBE", naam: "Levensbeschouwelijk"),
        new HoofdactiviteitVerenigingsloket(code: "MADP", naam: "Maatschappelijke dienstverlening & Politiek"),
        new HoofdactiviteitVerenigingsloket(code: "MEDG", naam: "Mediawijsheid & Digitale geletterdheid"),
        new HoofdactiviteitVerenigingsloket(code: "MROW", naam: "Mobiliteit, Ruimtelijke ordening & Wonen"),
        new HoofdactiviteitVerenigingsloket(code: "NMDW", naam: "Natuur, Milieu & Dierenwelzijn"),
        new HoofdactiviteitVerenigingsloket(code: "VORM", naam: "Onderwijs & Vorming"),
        new HoofdactiviteitVerenigingsloket(code: "SOVO", naam: "Sociaal-cultureel volwassenenwerk"),
        new HoofdactiviteitVerenigingsloket(code: "SPRT", naam: "Sport"),
        new HoofdactiviteitVerenigingsloket(code: "ONWE", naam: "Technologie & Wetenschap"),
        new HoofdactiviteitVerenigingsloket(code: "TOER", naam: "Toerisme"),
        new HoofdactiviteitVerenigingsloket(code: "WESE", naam: "Werk & Sociale economie"),
    };

    public static readonly int HoofdactiviteitenVerenigingsloketCount = HoofdactiviteitenVerenigingsloket.Count;

    private HoofdactiviteitVerenigingsloket(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Naam { get; }
    public string Code { get; }

    public static HoofdactiviteitVerenigingsloket Create(string key)
    {
        var value = HoofdactiviteitenVerenigingsloket.SingleOrDefault(p => p.Code == key);

        return value ?? throw new HoofdactiviteitCodeIsNietGekend(key);
    }

    public static IImmutableList<HoofdactiviteitVerenigingsloket> All()
        => HoofdactiviteitenVerenigingsloket.ToImmutableList();
}
