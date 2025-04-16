namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs : DomainException
{

    public VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs() : base(ExceptionMessages.VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs)
    {
    }

    protected VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
