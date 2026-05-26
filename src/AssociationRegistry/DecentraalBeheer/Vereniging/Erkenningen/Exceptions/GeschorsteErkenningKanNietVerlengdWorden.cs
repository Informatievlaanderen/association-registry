namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class GeschorsteErkenningKanNietVerlengdWorden : DomainException
{
    public GeschorsteErkenningKanNietVerlengdWorden()
        : base(ExceptionMessages.GeschorsteErkenningKanNietVerlengdWorden) { }

    protected GeschorsteErkenningKanNietVerlengdWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
