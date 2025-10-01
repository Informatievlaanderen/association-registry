UPDATE public.mt_events
SET type = 'kbo_nummer_werd_gereserveerd'
  , mt_dotnet_type = 'AssociationRegistry.Events.KboNummerWerdGereserveerd, AssociationRegistry'
WHERE type = '<>f__AnonymousType0<string>'
  AND mt_dotnet_type like '<>f__AnonymousType0`1[[System.String, System.Private.CoreLib, Version=%.0.0.0, Culture=neutral, PublicKeyToken=%]], AssociationRegistry%';
