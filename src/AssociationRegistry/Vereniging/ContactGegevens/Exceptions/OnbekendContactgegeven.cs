namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OnbekendContactgegeven : DomainException
{
    public OnbekendContactgegeven(string contactgegevenId) : base($"Contactgegeven met contactgegevenId '{contactgegevenId}' is niet gekend.")
    {
    }

    protected OnbekendContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
