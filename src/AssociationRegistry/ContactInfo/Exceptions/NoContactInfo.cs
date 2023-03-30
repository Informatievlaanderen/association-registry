namespace AssociationRegistry.ContactInfo.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class NoContactInfo : DomainException
{
    public NoContactInfo() : base("Een contact moet minstens één waarde bevatten.")
    {
    }

    protected NoContactInfo(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
