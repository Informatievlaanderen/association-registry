namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class AdresIdIsIncompleet : DomainException
{
    public AdresIdIsIncompleet() : base(ExceptionMessages.IncompleteAdresId)
    {
    }

    protected AdresIdIsIncompleet(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
