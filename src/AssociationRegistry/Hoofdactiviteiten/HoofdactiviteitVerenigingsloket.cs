namespace AssociationRegistry.Hoofdactiviteiten;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Exceptions;

public class HoofdactiviteitVerenigingsloket
{
    private static readonly List<HoofdactiviteitVerenigingsloket> Hoofdactiviteiten = new()
    {
        new HoofdactiviteitVerenigingsloket("BIAG", "Burgerinitiatief & Actiegroep"),
        new HoofdactiviteitVerenigingsloket("BWWC", "Buurtwerking & Wijkcomité"),
        new HoofdactiviteitVerenigingsloket("DINT", "Diversiteit & Integratie"),
        new HoofdactiviteitVerenigingsloket("GEWE", "Gezondheid & Welzijn"),
        new HoofdactiviteitVerenigingsloket("HOSP", "Hobby & Spel"),
        new HoofdactiviteitVerenigingsloket("INOS", "Internationaal & Onwikkelingssamenwerking"),
        new HoofdactiviteitVerenigingsloket("JGDW", "Jeugdwerk"),
        new HoofdactiviteitVerenigingsloket("KLDU", "Klimaat & Duurzaamheid"),
        new HoofdactiviteitVerenigingsloket("KECU", "Kunsten, Erfgoed & Cultuur"),
        new HoofdactiviteitVerenigingsloket("LAVI", "Landbouw & Visserij"),
        new HoofdactiviteitVerenigingsloket("LEBE", "Levenbeschouwelijk"),
        new HoofdactiviteitVerenigingsloket("MADP", "Maatschappelijke dienstverlening & Politiek"),
        new HoofdactiviteitVerenigingsloket("MEDG", "Mediawijsheid & Digitale geletterdheid"),
        new HoofdactiviteitVerenigingsloket("MROW", "Mobiliteit, Ruimtelijke ordening & Wonen"),
        new HoofdactiviteitVerenigingsloket("NMDW", "Natuur, Milieu & Dierenwelzijn"),
        new HoofdactiviteitVerenigingsloket("VORM", "Onderwijs & Vorming"),
        new HoofdactiviteitVerenigingsloket("SOVO", "Sociaal-cultureel volwassenenwerk"),
        new HoofdactiviteitVerenigingsloket("SPRT", "Sport"),
        new HoofdactiviteitVerenigingsloket("ONWE", "Technologie & Wetenschap"),
        new HoofdactiviteitVerenigingsloket("TOER", "Toerisme"),
        new HoofdactiviteitVerenigingsloket("WESE", "Werk & Sociale economie"),
    };

    public string Beschrijving { get; }
    public string Code { get; }

    private HoofdactiviteitVerenigingsloket(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public static HoofdactiviteitVerenigingsloket Create(string key)
    {
        var value = Hoofdactiviteiten.SingleOrDefault(p => p.Code == key);
        return value ?? throw new UnknownHoofdactiviteitCode(key);
    }

    public static IImmutableList<HoofdactiviteitVerenigingsloket> All()
        => Hoofdactiviteiten.ToImmutableList();
}
