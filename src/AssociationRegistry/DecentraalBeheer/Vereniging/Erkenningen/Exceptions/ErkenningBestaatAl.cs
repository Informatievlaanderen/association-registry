namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningBestaatAl : DomainException
{
    public ErkenningBestaatAl()
        : base(ExceptionMessages.ErkenningBestaatAl) { }

    protected ErkenningBestaatAl(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
