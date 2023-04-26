namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownVertegenwoordiger : DomainException
{
    public UnknownVertegenwoordiger() : base("Vertegenwoordiger is niet gekend.")
    {
    }

    protected UnknownVertegenwoordiger(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
