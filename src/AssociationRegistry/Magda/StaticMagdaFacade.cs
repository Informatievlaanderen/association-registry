namespace AssociationRegistry.Magda;

using ContactInfo;
using INSZ;
using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;

public class StaticMagdaFacade : IMagdaFacade
{
    public Task<IEnumerable<Vertegenwoordiger>?> GetVertegenwoordigers(IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger>? vertegenwoordigers, CancellationToken token = default)
        => Task.FromResult(
            vertegenwoordigers?.Select(
                v => Vertegenwoordiger.Create(
                    Insz.Create(v.Insz),
                    v.PrimairContactpersoon,
                    v.Roepnaam,
                    v.Rol,
                    "Jhon",
                    "Doo",
                    ContactLijst.Create(v.ContactInfoLijst))
            )
        );
}
