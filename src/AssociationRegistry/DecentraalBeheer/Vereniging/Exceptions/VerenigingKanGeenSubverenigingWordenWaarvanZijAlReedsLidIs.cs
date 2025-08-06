namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
