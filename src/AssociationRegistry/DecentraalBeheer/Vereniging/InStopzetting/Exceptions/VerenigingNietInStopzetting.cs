namespace AssociationRegistry.DecentraalBeheer.Vereniging.InStopzetting.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingNietInStopzetting : DomainException
{
    public VerenigingNietInStopzetting()
        : base(ExceptionMessages.VerenigingNietInStopzetting) { }

    protected VerenigingNietInStopzetting(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
