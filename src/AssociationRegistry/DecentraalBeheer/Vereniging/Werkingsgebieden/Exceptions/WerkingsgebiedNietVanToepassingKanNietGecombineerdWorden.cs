namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden : DomainException
{
    public WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden() : base(ExceptionMessages.WerkingsgebiedKanNietGecombineerdWordenMetNVT)
    {
    }

    protected WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
