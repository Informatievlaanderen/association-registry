namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
