namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class ContactgegevenIsNietGekend : DomainException
{
    public ContactgegevenIsNietGekend(string contactgegevenId) : base(
        $"Het opgegeven contactgegevenId '{contactgegevenId}' werd niet teruggevonden.")
    {
    }

    protected ContactgegevenIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
