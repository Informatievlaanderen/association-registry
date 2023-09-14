namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactgegevenIsDuplicaat : DomainException
{
    public ContactgegevenIsDuplicaat(string type) : base($"Er is reeds een {type} contactgegeven met de opgegeven waarde.")
    {
    }

    protected ContactgegevenIsDuplicaat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
