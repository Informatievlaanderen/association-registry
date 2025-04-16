namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs : DomainException
{

    public VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs() : base(ExceptionMessages.VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs)
    {
    }

    protected VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
