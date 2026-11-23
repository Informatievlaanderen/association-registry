namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>Only a vereniging zonder eigen rechtspersoonlijkheid without subtype or with subtype NB can become sub.</summary>
[Serializable]
public class SubverenigingsaanvraagIsNietToegestaan : DomainException
{
    public SubverenigingsaanvraagIsNietToegestaan()
        : base(ExceptionMessages.SubverenigingsaanvraagIsNietToegestaan) { }

    protected SubverenigingsaanvraagIsNietToegestaan(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
