namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensQuery :
    IQuery<VertegenwoordigerPersoonsgegevensDocument?, VertegenwoordigerPersoonsgegevensByRefIdFilter>,
    IQuery<VertegenwoordigerPersoonsgegevensDocument[], VertegenwoordigerPersoonsgegevensByRefIdsFilter>,
    IQuery<VertegenwoordigerPersoonsgegevensDocument[], VertegenwoordigerPersoonsgegevensByInszFilter>;
