namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultiplePrimairContactgegevens : DomainException
{
    public MultiplePrimairContactgegevens(string type) : base($"Er mag maar één {type} contactgegeven aangeduid zijn als primair.")
    {
    }

    protected MultiplePrimairContactgegevens(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
