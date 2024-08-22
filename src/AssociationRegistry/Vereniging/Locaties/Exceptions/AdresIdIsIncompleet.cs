namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
