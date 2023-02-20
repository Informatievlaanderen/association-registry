namespace AssociationRegistry.ContactInfo.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultiplePrimaryContactInfos : DomainException
{
    public MultiplePrimaryContactInfos() : base("Er mag maar één contactInfo aangeduid zijn als primairContactInfo.")
    {
    }

    protected MultiplePrimaryContactInfos(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
