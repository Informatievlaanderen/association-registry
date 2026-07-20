namespace AssociationRegistry.DecentraalBeheer.Vereniging.InStopzetting.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingReedsInStopzetting : DomainException
{
    public VerenigingReedsInStopzetting()
        : base(ExceptionMessages.VerenigingReedsInStopzetting) { }

    protected VerenigingReedsInStopzetting(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
