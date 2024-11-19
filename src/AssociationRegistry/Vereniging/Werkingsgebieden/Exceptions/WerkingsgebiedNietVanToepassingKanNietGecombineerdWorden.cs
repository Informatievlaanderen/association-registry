namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden : DomainException
{
    public WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden() : base($"Het opgegeven werkingsgebied NVT kan niet gecombineerd worden met andere codes")
    {
    }

    protected WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
