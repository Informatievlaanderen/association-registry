namespace AssociationRegistry.Vertegenwoordigers.Exceptions;

using System.Runtime.Serialization;

public class MultiplePrimaryContacts : Exception
{
    public MultiplePrimaryContacts() : base("Er mag maar één vertegenwoordiger aangeduid zijn als primair contactpersoon.")
    {
    }

    protected MultiplePrimaryContacts(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
