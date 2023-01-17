namespace AssociationRegistry.VerenigingsNamen;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class VerenigingsNaam : ValueObject<VerenigingsNaam>, IEquatable<string>
{
    private readonly string _naam;

    public VerenigingsNaam(string naam)
    {
        Throw<EmptyVerenigingsNaam>.IfNullOrWhiteSpace(naam);
        _naam = naam;
    }

    public bool Equals(string? other)
        => _naam.Equals(other);

    public override string ToString()
        => _naam;

    protected override IEnumerable<object> Reflect()
    {
        yield return _naam;
    }
}
