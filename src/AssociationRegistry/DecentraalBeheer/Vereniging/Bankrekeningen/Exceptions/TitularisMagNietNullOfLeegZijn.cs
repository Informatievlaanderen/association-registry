namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class TitularisMagNietNullOfLeegZijn: DomainException
{
    public TitularisMagNietNullOfLeegZijn() : base(ExceptionMessages.TitularisMagNietNullOfLeegZijn)
    {
    }

    protected TitularisMagNietNullOfLeegZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
