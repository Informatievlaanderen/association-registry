namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class TitularissenMoetenUniekZijn : DomainException
{
    public TitularissenMoetenUniekZijn()
        : base(ExceptionMessages.TitularissenMoetenUniekZijn) { }

    protected TitularissenMoetenUniekZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
