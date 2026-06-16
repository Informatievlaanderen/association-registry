namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningCombinatieBestaatAl : DomainException
{
    public ErkenningCombinatieBestaatAl()
        : base(ExceptionMessages.ErkenningBestaatAl) { }

    protected ErkenningCombinatieBestaatAl(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
