namespace AssociationRegistry.Contactgegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OnbekendContactgegeven : DomainException
{
    public OnbekendContactgegeven(string contactgegevenId) : base($"Het opgegeven contactgegevenId '{contactgegevenId}' werd niet teruggevonden.")
    {
    }

    protected OnbekendContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
