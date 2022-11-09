namespace AssociationRegistry.Admin.Api;

using System;

public interface IClock
{
    DateOnly Today { get; }
}
