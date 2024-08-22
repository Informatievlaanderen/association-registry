namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MaatschappelijkeZetelIsNietToegestaan : DomainException
{
    public MaatschappelijkeZetelIsNietToegestaan() : base(ExceptionMessages.MaatschappelijkeZetelIsNotAllowed)
    {
    }

    protected MaatschappelijkeZetelIsNietToegestaan(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
