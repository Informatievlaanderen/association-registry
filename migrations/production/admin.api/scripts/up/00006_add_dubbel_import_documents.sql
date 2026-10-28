DROP TABLE IF EXISTS public.mt_doc_dubbelimportbatchdocument CASCADE;
CREATE TABLE public.mt_doc_dubbelimportbatchdocument (
    id                  uuid                        NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
    mt_dotnet_type      varchar                     NULL,
CONSTRAINT pkey_mt_doc_dubbelimportbatchdocument_id PRIMARY KEY (id)
);

CREATE OR REPLACE FUNCTION public.mt_upsert_dubbelimportbatchdocument(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_dubbelimportbatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where public.mt_doc_dubbelimportbatchdocument.mt_version = current_version;

  SELECT mt_version FROM public.mt_doc_dubbelimportbatchdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_dubbelimportbatchdocument(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_dubbelimportbatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_dubbelimportbatchdocument(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_dubbelimportbatchdocument SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where public.mt_doc_dubbelimportbatchdocument.mt_version = current_version and id = docId;

  SELECT mt_version FROM public.mt_doc_dubbelimportbatchdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_overwrite_dubbelimportbatchdocument(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_dubbelimportbatchdocument ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_dubbelimportbatchdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;

DROP TABLE IF EXISTS public.mt_doc_dubbelimportdocument CASCADE;
CREATE TABLE public.mt_doc_dubbelimportdocument (
    id                  uuid                        NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_dotnet_type      varchar                     NULL,
    mt_version          integer                     NOT NULL DEFAULT 0,
    batch_id            uuid                        NULL,
CONSTRAINT pkey_mt_doc_dubbelimportdocument_id PRIMARY KEY (id)
);

CREATE INDEX mt_doc_dubbelimportdocument_idx_batch_id ON public.mt_doc_dubbelimportdocument USING btree (batch_id);

CREATE OR REPLACE FUNCTION public.mt_upsert_dubbelimportdocument(arg_batch_id uuid, doc JSONB, docDotNetType varchar, docId uuid, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

SELECT mt_version into current_version FROM public.mt_doc_dubbelimportdocument WHERE id = docId ;
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

INSERT INTO public.mt_doc_dubbelimportdocument ("batch_id", "data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (arg_batch_id, doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "batch_id" = arg_batch_id, "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_dubbelimportdocument.mt_version;

  SELECT mt_version into final_version FROM public.mt_doc_dubbelimportdocument WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_dubbelimportdocument(arg_batch_id uuid, doc JSONB, docDotNetType varchar, docId uuid, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_dubbelimportdocument ("batch_id", "data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (arg_batch_id, doc, docDotNetType, docId, revision, transaction_timestamp());
  RETURN 1;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_dubbelimportdocument(arg_batch_id uuid, doc JSONB, docDotNetType varchar, docId uuid, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN
  if revision <= 1 then
    SELECT mt_version FROM public.mt_doc_dubbelimportdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    end if;
  end if;

  UPDATE public.mt_doc_dubbelimportdocument SET "batch_id" = arg_batch_id, "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp() where revision > public.mt_doc_dubbelimportdocument.mt_version and id = docId;

  SELECT mt_version FROM public.mt_doc_dubbelimportdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_overwrite_dubbelimportdocument(arg_batch_id uuid, doc JSONB, docDotNetType varchar, docId uuid, revision integer) RETURNS INTEGER LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version INTEGER;
  current_version INTEGER;
BEGIN

  if revision = 0 then
    SELECT mt_version FROM public.mt_doc_dubbelimportdocument into current_version WHERE id = docId ;
    if current_version is not null then
      revision = current_version + 1;
    else
      revision = 1;
    end if;
  end if;

  INSERT INTO public.mt_doc_dubbelimportdocument ("batch_id", "data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (arg_batch_id, doc, docDotNetType, docId, revision, transaction_timestamp())
  ON CONFLICT (id)
  DO UPDATE SET "batch_id" = arg_batch_id, "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = revision, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_dubbelimportdocument into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;

