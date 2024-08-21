namespace AssociationRegistry.Admin.Api.Verenigingen.Search.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System;

[Serializable]
public class ZoekOpdrachtWasIncorrect : DomainException
{
    public ZoekOpdrachtWasIncorrect() : base(ExceptionMessages.ZoekOpdrachtWasIncorrect)
    {
    }
}
