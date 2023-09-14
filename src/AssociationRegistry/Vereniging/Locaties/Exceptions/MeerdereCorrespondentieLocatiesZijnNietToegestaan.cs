namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
