DROP FUNCTION IF EXISTS public.mt_upsert_locatiezonderadresmatchdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) cascade;

CREATE OR REPLACE FUNCTION public.mt_upsert_locatiezonderadresmatchdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_locatiezonderadresmatchdocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_locatiezonderadresmatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_locatiezonderadresmatchdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_locatiezonderadresmatchdocument WHERE id = docId ;
  RETURN final_version;
END;
$function$;

DROP FUNCTION IF EXISTS public.mt_upsert_powerbiexportdocument(doc jsonb, docdotnettype character varying, docid character varying, revision integer) cascade;

CREATE OR REPLACE FUNCTION public.mt_upsert_powerbiexportdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_powerbiexportdocument WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version + 1;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version >= revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_powerbiexportdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_powerbiexportdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_powerbiexportdocument WHERE id = docId ;
  RETURN final_version;
END;
$function$;

