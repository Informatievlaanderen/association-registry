namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactTypeIsOngeldig : DomainException
{
    public ContactTypeIsOngeldig() : base(ExceptionMessages.InvalidContactType)
    {
    }

    protected ContactTypeIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
