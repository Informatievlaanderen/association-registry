namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record RechtsvormWerdGewijzigdInKBO(string Rechtsvorm) : IEvent
{
    public static RechtsvormWerdGewijzigdInKBO With(Verenigingstype verenigingstype)
        => new(verenigingstype.Code);
}
