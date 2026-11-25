namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>vr-namens is the ontvanger, or no partij of the relatieaanvraag.</summary>
[Serializable]
public class NamensIsNietDeAanvrager : DomainException
{
    public NamensIsNietDeAanvrager()
        : base(ExceptionMessages.NamensIsNietDeAanvrager) { }

    protected NamensIsNietDeAanvrager(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
