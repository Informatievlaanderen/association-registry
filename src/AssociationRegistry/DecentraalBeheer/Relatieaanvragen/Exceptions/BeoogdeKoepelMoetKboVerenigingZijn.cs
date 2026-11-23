namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class BeoogdeKoepelMoetKboVerenigingZijn : DomainException
{
    public BeoogdeKoepelMoetKboVerenigingZijn()
        : base(ExceptionMessages.BeoogdeKoepelMoetKboVerenigingZijn) { }

    protected BeoogdeKoepelMoetKboVerenigingZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
