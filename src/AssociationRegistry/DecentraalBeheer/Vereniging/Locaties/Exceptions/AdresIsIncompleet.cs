namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
