namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class ContactgegevenIsNietGekend : DomainException
{
    public ContactgegevenIsNietGekend(string contactgegevenId) : base($"Het opgegeven contactgegevenId '{contactgegevenId}' werd niet teruggevonden.")
    {
    }

    protected ContactgegevenIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
