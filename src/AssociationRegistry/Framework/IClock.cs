namespace AssociationRegistry.Framework;

using System;

public interface IClock
{
    DateOnly Today { get; }
}
