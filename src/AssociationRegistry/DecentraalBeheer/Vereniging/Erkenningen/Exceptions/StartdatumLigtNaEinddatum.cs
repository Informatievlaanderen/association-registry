namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class HernieuwingsDatumMoetTussenStartEnEindDatumLiggen : DomainException
{
    public HernieuwingsDatumMoetTussenStartEnEindDatumLiggen() : base(ExceptionMessages.HernieuwingsDatumMoetTussenStartEnEindDatumLiggen)
    {
    }

    protected HernieuwingsDatumMoetTussenStartEnEindDatumLiggen(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
