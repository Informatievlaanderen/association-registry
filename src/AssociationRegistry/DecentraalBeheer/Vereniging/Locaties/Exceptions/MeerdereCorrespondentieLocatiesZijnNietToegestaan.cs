namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class MeerdereCorrespondentieLocatiesZijnNietToegestaan : DomainException
{
    public MeerdereCorrespondentieLocatiesZijnNietToegestaan() : base(ExceptionMessages.MultipleCorrespondentieLocaties)
    {
    }

    protected MeerdereCorrespondentieLocatiesZijnNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
