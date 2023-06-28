namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class MultiplePrimaireVertegenwoordigers : DomainException
{
    public MultiplePrimaireVertegenwoordigers() : base("Er mag maar één vertegenwoordiger aangeduid zijn als primair contactpersoon.")
    {
    }

    protected MultiplePrimaireVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
