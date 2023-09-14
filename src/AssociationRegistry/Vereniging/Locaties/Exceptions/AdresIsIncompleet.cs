namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AdresIsIncompleet : DomainException
{
    public AdresIsIncompleet() : base(ExceptionMessages.IncompleteAdres)
    {
    }

    protected AdresIsIncompleet(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
