namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class MultiplePrimaryContacts : DomainException
{
    public MultiplePrimaryContacts() : base("Er mag maar één vertegenwoordiger aangeduid zijn als primair contactpersoon.")
    {
    }

    protected MultiplePrimaryContacts(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
