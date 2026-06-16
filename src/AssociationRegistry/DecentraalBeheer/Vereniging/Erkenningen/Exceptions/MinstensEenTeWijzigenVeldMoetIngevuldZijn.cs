namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class MinstensEenTeWijzigenVeldMoetIngevuldZijn : DomainException
{
    public MinstensEenTeWijzigenVeldMoetIngevuldZijn()
        : base(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn) { }

    protected MinstensEenTeWijzigenVeldMoetIngevuldZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
