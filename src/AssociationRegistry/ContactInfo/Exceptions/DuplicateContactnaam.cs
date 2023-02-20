namespace AssociationRegistry.ContactInfo.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateContactnaam: DomainException
{
    public DuplicateContactnaam() : base("Contactnaam moet uniek zijn.")
    {
    }

    protected DuplicateContactnaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
