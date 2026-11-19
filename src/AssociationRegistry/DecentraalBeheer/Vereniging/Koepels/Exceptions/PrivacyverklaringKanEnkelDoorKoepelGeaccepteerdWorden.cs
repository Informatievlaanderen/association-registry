namespace AssociationRegistry.DecentraalBeheer.Vereniging.Koepels.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden : DomainException
{
    public PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden()
        : base(ExceptionMessages.PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden) { }

    protected PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
