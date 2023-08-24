namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactgegevenFromKboCannotBeRemoved : DomainException
{
    public ContactgegevenFromKboCannotBeRemoved() : base(ExceptionMessages.ContactgegevenFromKboCannotBeRemoved)
    {
    }

    protected ContactgegevenFromKboCannotBeRemoved(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
