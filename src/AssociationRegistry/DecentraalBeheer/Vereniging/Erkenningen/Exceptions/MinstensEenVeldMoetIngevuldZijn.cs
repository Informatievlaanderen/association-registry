namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class MinstensEenVeldMoetIngevuldZijn : DomainException
{
    public MinstensEenVeldMoetIngevuldZijn()
        : base(ExceptionMessages.MinstensEenVeldMoetIngevuldZijn) { }

    protected MinstensEenVeldMoetIngevuldZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
