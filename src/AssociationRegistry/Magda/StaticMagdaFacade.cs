namespace AssociationRegistry.Magda;

using INSZ;
using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;

public class StaticMagdaFacade : IMagdaFacade
{
    public Task<VertegenwoordigersLijst> GetVertegenwoordigers(IEnumerable<RegistreerVerenigingCommand.Vertegenwoordiger> vertegenwoordigers, CancellationToken token = default)
        => Task.FromResult(
            VertegenwoordigersLijst.Create(
                vertegenwoordigers.Select(
                    v => Vertegenwoordiger.Create(
                        Insz.Create(v.Insz),
                        v.PrimairContactpersoon,
                        v.Roepnaam,
                        v.Rol,
                        "Jhon",
                        "Doo")
                )
            )
        );
}
