﻿namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MaatschappelijkeZetelKanNietGewijzigdWorden : DomainException
{
    public MaatschappelijkeZetelKanNietGewijzigdWorden() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeUpdated)
    {
    }

    protected MaatschappelijkeZetelKanNietGewijzigdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
