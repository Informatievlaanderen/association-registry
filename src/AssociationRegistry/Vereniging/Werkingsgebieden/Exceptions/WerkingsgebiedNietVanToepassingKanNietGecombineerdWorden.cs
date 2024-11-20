namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
