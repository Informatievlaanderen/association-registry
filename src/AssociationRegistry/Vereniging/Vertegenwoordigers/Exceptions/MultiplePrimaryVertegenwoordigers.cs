namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class MultiplePrimaryVertegenwoordigers : DomainException
{
    public MultiplePrimaryVertegenwoordigers() : base("Er mag maar één vertegenwoordiger aangeduid zijn als primair contactpersoon.")
    {
    }

    protected MultiplePrimaryVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
