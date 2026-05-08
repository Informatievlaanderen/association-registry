namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerlopenErkenningKanNietVerwijderdWorden : DomainException
{
    public VerlopenErkenningKanNietVerwijderdWorden()
        : base(ExceptionMessages.VerlopenErkenningKanNietVerwijderdWorden) { }

    protected VerlopenErkenningKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
