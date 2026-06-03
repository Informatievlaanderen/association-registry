DROP TABLE IF EXISTS public.mt_doc_erkenningdocument CASCADE;
CREATE TABLE public.mt_doc_erkenningdocument (
    id                  varchar                     NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_dotnet_type      varchar                     NULL,
    mt_version          integer                     NOT NULL DEFAULT 0,
CONSTRAINT pkey_mt_doc_erkenningdocument_id PRIMARY KEY (id)
);

CREATE OR REPLACE FUNCTION public.mt_upsert_erkenningdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_erkenningdocument WHERE id = docId ;
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

INSERT INTO public.mt_doc_erkenningdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_erkenningdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_erkenningdocument WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_erkenningdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_erkenningdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_erkenningdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_erkenningdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_erkenningdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_erkenningdocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_erkenningdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_overwrite_erkenningdocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_erkenningdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_erkenningdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_erkenningdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;

