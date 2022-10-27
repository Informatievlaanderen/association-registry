namespace AssociationRegistry.Admin.Api.Verenigingen.VerenigingsNamen;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class VerenigingsNaam : ValueObject<VerenigingsNaam>
{
    private readonly string _naam;

    public VerenigingsNaam(string naam)
    {
        Throw<EmptyVerenigingsNaam>.IfNullOrWhiteSpace(naam);
        _naam = naam;
    }

    public override string ToString()
        => _naam;

    protected override IEnumerable<object> Reflect()
    {
        yield return _naam;
    }
}
