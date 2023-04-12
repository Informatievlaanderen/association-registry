namespace AssociationRegistry.Contactgegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultiplePrimaryContactgegevens : DomainException
{
    public MultiplePrimaryContactgegevens(string type) : base($"Er mag maar één {type} contactgegeven aangeduid zijn als primair.")
    {
    }

    protected MultiplePrimaryContactgegevens(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
