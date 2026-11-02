namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.Events;

/// <summary>Bestaande hoofdactiviteiten uit de lijst van het verenigingsloket.</summary>
public static class TestHoofdactiviteiten
{
    public static readonly Registratiedata.HoofdactiviteitVerenigingsloket Sport = new("SPRT", "Sport");
    public static readonly Registratiedata.HoofdactiviteitVerenigingsloket Jeugdwerk = new("JGDW", "Jeugdwerk");
    public static readonly Registratiedata.HoofdactiviteitVerenigingsloket Toerisme = new("TOER", "Toerisme");
}

public static class TestWerkingsgebieden
{
    public static readonly Registratiedata.Werkingsgebied VlaamsBrabant = new("BE24", "Provincie Vlaams-Brabant");
    public static readonly Registratiedata.Werkingsgebied Antwerpen = new("BE21", "Provincie Antwerpen");
    public static readonly Registratiedata.Werkingsgebied NietVanToepassing = new("NVT", "Niet van toepassing");
}

public static class TestCodesExtensions
{
    public static FeitelijkeVerenigingWerdGeregistreerd WithHoofdactiviteiten(
        this FeitelijkeVerenigingWerdGeregistreerd @event,
        params Registratiedata.HoofdactiviteitVerenigingsloket[] hoofdactiviteiten
    ) => @event with { HoofdactiviteitenVerenigingsloket = hoofdactiviteiten };

    public static VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd WithHoofdactiviteiten(
        this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event,
        params Registratiedata.HoofdactiviteitVerenigingsloket[] hoofdactiviteiten
    ) => @event with { HoofdactiviteitenVerenigingsloket = hoofdactiviteiten };
}
