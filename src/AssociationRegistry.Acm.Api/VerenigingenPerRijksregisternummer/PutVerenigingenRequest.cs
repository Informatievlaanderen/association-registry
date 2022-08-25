namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

using System.Collections.Generic;

public class PutVerenigingenRequest
{
    
    public List<Vereniging> Verenigingen { get; set; } = null!;

    public class Vereniging
    {
        public string Id { get; set; } = null!;
        public string Naam { get; set; } = null!;
    }
}
