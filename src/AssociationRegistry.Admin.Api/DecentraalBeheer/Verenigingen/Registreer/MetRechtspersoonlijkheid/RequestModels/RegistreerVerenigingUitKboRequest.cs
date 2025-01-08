namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;

using Acties.Registratie.RegistreerVerenigingUitKbo;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

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
