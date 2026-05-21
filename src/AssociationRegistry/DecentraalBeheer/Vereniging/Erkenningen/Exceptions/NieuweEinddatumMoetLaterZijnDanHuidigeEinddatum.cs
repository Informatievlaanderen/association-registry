namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum : DomainException
{
    public NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum() : base(ExceptionMessages.NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum)
    {
    }

    protected NieuweEinddatumMoetLaterZijnDanHuidigeEinddatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
