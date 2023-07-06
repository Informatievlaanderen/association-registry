namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidDoelgroepRange : DomainException
{
    public InvalidDoelgroepRange() : base("Minimum leeftijd moet kleiner of gelijk zijn aan maximum leeftijd.")
    {
    }

    protected InvalidDoelgroepRange(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
