namespace AssociationRegistry.Admin.Api.Verenigingen.MetRechtspersoonlijkheid.Registreer;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Acties.RegistreerVerenigingUitKbo;

[DataContract]
public class RegistreerVerenigingUitKboRequest
{
    /// <summary>Kbo nummer van de vereniging</summary>
    [DataMember]
    [Required]
    public string KboNummer { get; init; } = null!;

    public RegistreerVerenigingUitKboCommand ToCommand()
        => new(Vereniging.KboNummer.Create(KboNummer));
}
