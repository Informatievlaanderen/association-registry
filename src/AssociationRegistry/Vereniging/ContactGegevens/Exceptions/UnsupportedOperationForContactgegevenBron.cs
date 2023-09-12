namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class UnsupportedOperationForContactgegevenBron : DomainException
{
    public UnsupportedOperationForContactgegevenBron() : base(ExceptionMessages.UnsupportedOperationForContactgegevenBron)
    {
    }

    protected UnsupportedOperationForContactgegevenBron(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
