namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownLocatie : DomainException
{
    public UnknownLocatie() : base("Locatie is niet gekend")
    {
    }

    protected UnknownLocatie(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
