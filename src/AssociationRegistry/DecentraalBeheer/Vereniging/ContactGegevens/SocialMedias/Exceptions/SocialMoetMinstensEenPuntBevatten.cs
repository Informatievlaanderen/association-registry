namespace AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class SocialMoetMinstensEenPuntBevatten : DomainException
{
    public SocialMoetMinstensEenPuntBevatten() : base(ExceptionMessages.SocialMediaMissingPeriod)
    {
    }

    protected SocialMoetMinstensEenPuntBevatten(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
