namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class IbanMoetUniekZijn: DomainException
{
    public IbanMoetUniekZijn() : base(ExceptionMessages.IbanMoetUniekZijn)
    {
    }

    protected IbanMoetUniekZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
