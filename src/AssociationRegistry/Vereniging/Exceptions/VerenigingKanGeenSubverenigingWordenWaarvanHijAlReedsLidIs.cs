namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIs : DomainException
{

    public VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIs() : base(ExceptionMessages.VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIs)
    {
    }

    protected VerenigingKanGeenSubverenigingWordenWaarvanHijAlReedsLidIs(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
