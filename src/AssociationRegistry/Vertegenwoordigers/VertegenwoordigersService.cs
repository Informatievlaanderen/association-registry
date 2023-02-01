namespace AssociationRegistry.Vertegenwoordigers;

using INSZ;

public class VertegenwoordigersService
{
    public Vertegenwoordiger CreateVertegenwoordiger(Insz insz, string roepnaam, string rol, bool primairContactpersoon)
        => Vertegenwoordiger.Create(insz, primairContactpersoon, roepnaam, rol, "", "");
}
