namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;

public interface IBankrekeningnummerPersoonsgegevensQuery :
    IQuery<BankrekeningnummerPersoonsgegevensDocument?, BankrekeningnummerPersoonsgegevensByRefIdFilter>,
    IQuery<BankrekeningnummerPersoonsgegevensDocument[], BankrekeningnummerPersoonsgegevensByRefIdsFilter>,
    IQuery<BankrekeningnummerPersoonsgegevensDocument[], BankrekeningnummerPersoonsgegevensByIbanFilter>;
