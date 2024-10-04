namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class WerkingsgebiedCodeIsNietGekend : DomainException
{
    public WerkingsgebiedCodeIsNietGekend(string unknownCode) : base($"Het opgegeven werkingsgebied is niet gekend: \"{unknownCode}\"")
    {
    }

    protected WerkingsgebiedCodeIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
