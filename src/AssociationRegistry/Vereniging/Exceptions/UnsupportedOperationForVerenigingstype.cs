namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnsupportedOperationForVerenigingstype : DomainException
{
    public UnsupportedOperationForVerenigingstype() : base(ExceptionMessages.UnsupportedOperationForVerenigingstype)
    {
    }

    protected UnsupportedOperationForVerenigingstype(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
