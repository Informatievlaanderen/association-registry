namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
