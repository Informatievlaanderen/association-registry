namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Subtype;
using EventFactories;
using Events;
using Exceptions;
using Framework;

public record SubverenigingSubtype : IVerenigingssubtype
{
    public SubverenigingVan SubverenigingVan { get; }

    public IVerenigingssubtypeCode Code => VerenigingssubtypeCodering.SubverenigingVan;

    public SubverenigingSubtype(SubverenigingVan subverenigingVan)
    {
        SubverenigingVan = subverenigingVan;
    }
    public IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event)
        => new SubverenigingSubtype(AssociationRegistry.Vereniging.SubverenigingVan.Hydrate(@event));

    public IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event)
        => new SubverenigingSubtype(AssociationRegistry.Vereniging.SubverenigingVan.Hydrate(@event));

    public IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode)
        => [EventFactory.SubtypeWerdVerfijndNaarFeitelijkeVereniging(vCode)];

    public IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode)
        => [EventFactory.SubtypeWerdTerugGezetNaarNietBepaald(vCode)];

    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
    {
        Throw<WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben>.If(
            TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(subverenigingVan));

        return SubverenigingVan.Wijzig(vCode, subverenigingVan);
    }

    private bool TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
        => subverenigingVan.AndereVereniging is null && subverenigingVan.Identificatie is null && subverenigingVan.Beschrijving is null;
    public bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging)
        => lidmaatschapAndereVereniging == SubverenigingVan.AndereVereniging;
}

// public class Verenigingssubtype : IHasVerenigingssubtypeCodeAndNaam
// {
//     public static class Codes
//     {
//         public const string FV = "FV";
//         public const string Sub = "SUB";
//         public const string NB = "NB";
//
//     }
//     public static readonly IVerenigingssubtype FeitelijkeVereniging = new Verenigingssubtype(code: Codes.FV, naam: "Feitelijke vereniging");
//     public static readonly IVerenigingssubtype Subvereniging = new Verenigingssubtype(code: Codes.Sub, naam: "Subvereniging");
//     public static readonly IVerenigingssubtype NietBepaald = new Verenigingssubtype(code: Codes.NB, naam: "Niet bepaald");
//
//     public static readonly IVerenigingssubtype[] All =
//     {
//         FeitelijkeVereniging,
//         Subvereniging,
//         NietBepaald,
//     };
//
//     public static readonly Verenigingssubtype Default = new (string.Empty, string.Empty);
//
//     public Verenigingssubtype(string code, string naam)
//     {
//         Code = code;
//         Naam = naam;
//     }
//
//     public string Code { get; init; }
//     public string Naam { get; init; }
//
//     public static IVerenigingssubtype Parse(string code)
//         => All.Single(t => t.Code == code);
//
//     public static string GetNameOrDefaultOrNull(string code)
//         => code switch
//         {
//             null => throw new ArgumentNullException(nameof(code)),
//             "" => Default.Naam,
//             _ => All.Single(t => t.Code == code).Naam,
//         };
//
//     public static bool IsValidSubtypeCode(string code)
//         => All.Any(t => t.Code == code);
//
//     public static bool IsGeenSubVereniging(string code)
//         => code != Subvereniging.Code;
//
//     public bool IsFeitelijkeVereniging
//         => Code == FeitelijkeVereniging.Code;
//
//     public bool IsNietBepaald
//         => Code == NietBepaald.Code;
//
//     public bool IsSubVereniging
//         => Code == Subvereniging.Code;
//
//     // private record DefaultVerenigingssubtype : IVerenigingssubtype
//     // {
//     //     public DefaultVerenigingssubtype()
//     //     {
//     //         Code = string.Empty;
//     //         Naam = string.Empty;
//     //     }
//     //     public string Code { get; init; }
//     //     public string Naam { get; init; }
//     // }
// }
