DROP TABLE IF EXISTS public.mt_doc_vertegenwoordigerspervcodedocument CASCADE;
CREATE TABLE public.mt_doc_vertegenwoordigerspervcodedocument (
    id                  varchar                     NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_dotnet_type      varchar                     NULL,
    mt_version          integer                     NOT NULL DEFAULT 0,
CONSTRAINT pkey_mt_doc_vertegenwoordigerspervcodedocument_id PRIMARY KEY (id)
);

CREATE OR REPLACE FUNCTION public.mt_upsert_vertegenwoordigerspervcodedocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT version into current_version FROM public.mt_streams WHERE id = docId ;
if revision = 0 then
  if current_version is not null then
    revision = current_version;
  else
    revision = 1;
  end if;
else
  if current_version is not null then
    if current_version > revision then
      return 0;
    end if;
  end if;
end if;

INSERT INTO public.mt_doc_vertegenwoordigerspervcodedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_vertegenwoordigerspervcodedocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_vertegenwoordigerspervcodedocument WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_vertegenwoordigerspervcodedocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_vertegenwoordigerspervcodedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_vertegenwoordigerspervcodedocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_vertegenwoordigerspervcodedocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_vertegenwoordigerspervcodedocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_vertegenwoordigerspervcodedocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_vertegenwoordigerspervcodedocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_overwrite_vertegenwoordigerspervcodedocument(doc JSONB, docDotNetType varchar, docId varchar, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_vertegenwoordigerspervcodedocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_vertegenwoordigerspervcodedocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_vertegenwoordigerspervcodedocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;

