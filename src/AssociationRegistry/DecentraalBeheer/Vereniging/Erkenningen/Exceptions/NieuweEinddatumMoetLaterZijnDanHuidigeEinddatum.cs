namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum : DomainException{

    public NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum() : base(ExceptionMessages.NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum)
    {
    }

    protected NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
