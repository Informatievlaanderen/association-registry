namespace AssociationRegistry.ContactInfo.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class NoContactnaam : DomainException
{
    public NoContactnaam() : base("Een contact moet eenn contactnaam bevatten.")
    {
    }

    protected NoContactnaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
