namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MissingAdres : DomainException
{
    public MissingAdres() : base(ExceptionMessages.MissingAdres)
    {
    }

    protected MissingAdres(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
