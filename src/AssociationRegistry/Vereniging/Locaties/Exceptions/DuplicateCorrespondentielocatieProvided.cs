namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateCorrespondentielocatieProvided : DomainException
{
    public DuplicateCorrespondentielocatieProvided() : base("Er kan maar één correspondentie locatie zijn binnen de vereniging.")
    {
    }

    protected DuplicateCorrespondentielocatieProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}