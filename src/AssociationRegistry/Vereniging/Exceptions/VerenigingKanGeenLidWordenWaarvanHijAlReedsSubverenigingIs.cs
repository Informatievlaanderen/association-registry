namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenLidWordenWaarvanHijAlReedsSubverenigingIs : DomainException
{

    public VerenigingKanGeenLidWordenWaarvanHijAlReedsSubverenigingIs() : base(ExceptionMessages.VerenigingKanGeenLidWordenWaarvanZijSubverenigingIs)
    {
    }

    protected VerenigingKanGeenLidWordenWaarvanHijAlReedsSubverenigingIs(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
