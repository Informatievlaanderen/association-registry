namespace AssociationRegistry.KboNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public abstract class InvalidKboNummer : DomainException
{
    protected InvalidKboNummer(string message) : base(message)
    {
    }

    protected InvalidKboNummer(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
