namespace AssociationRegistry.ContactGegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class OnbekendContactgegeven : AggregateNotFoundException
{
    public OnbekendContactgegeven(string contactgegevenId) : base(contactgegevenId, typeof(Contactgegeven))
    {
    }

    protected OnbekendContactgegeven(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
