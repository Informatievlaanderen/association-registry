namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.Events;

/// <summary>
///     De gegevens van een vereniging die meetellen voor de samenvoeging van een dubbele vereniging.
///     Werkingsgebieden worden apart bepaald na de registratie; leeg betekent niet bepaald.
/// </summary>
public record TestVerenigingsgegevens(
    TestBasisgegevens Basisgegevens,
    Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten,
    Registratiedata.Werkingsgebied[] Werkingsgebieden
);
