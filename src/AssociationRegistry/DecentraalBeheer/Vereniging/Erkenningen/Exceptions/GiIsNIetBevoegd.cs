namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class GiIsNIetBevoegd : DomainException
{
    public GiIsNIetBevoegd()
        : base(ExceptionMessages.GiIsNIetBevoegd) { }

    protected GiIsNIetBevoegd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
