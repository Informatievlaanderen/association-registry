namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class TitularisMoetUniekZijn : DomainException
{
    public TitularisMoetUniekZijn()
        : base(ExceptionMessages.IbanMoetUniekZijn) { }

    protected TitularisMoetUniekZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
