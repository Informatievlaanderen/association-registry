namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactgegevenFromKboCannotBeUpdated : DomainException
{
    public ContactgegevenFromKboCannotBeUpdated() : base(ExceptionMessages.ContactgegevenFromKboCannotBeUpdated)
    {
    }

    protected ContactgegevenFromKboCannotBeUpdated(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
