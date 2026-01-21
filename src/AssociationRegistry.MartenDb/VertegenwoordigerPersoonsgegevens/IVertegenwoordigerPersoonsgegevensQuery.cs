namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;

public interface IVertegenwoordigerPersoonsgegevensQuery :
    IQuery<VertegenwoordigerPersoonsgegevensDocument?, VertegenwoordigerPersoonsgegevensByRefIdFilter>,
    IQuery<VertegenwoordigerPersoonsgegevensDocument[], VertegenwoordigerPersoonsgegevensByRefIdsFilter>,
    IQuery<VertegenwoordigerPersoonsgegevensDocument[], VertegenwoordigerPersoonsgegevensByInszFilter>;
