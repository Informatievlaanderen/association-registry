namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class GiIsNietBevoegd : DomainException
{
    public GiIsNietBevoegd()
        : base(ExceptionMessages.GiIsNietBevoegd) { }

    protected GiIsNietBevoegd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
