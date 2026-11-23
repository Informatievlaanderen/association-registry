CREATE TABLE IF NOT EXISTS public.mt_doc_openrelatieaanvraagdocument (
    id                  varchar                     NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
    mt_dotnet_type      varchar                     NULL,
CONSTRAINT pkey_mt_doc_openrelatieaanvraagdocument_id PRIMARY KEY (id)
);
CREATE SEQUENCE public.mt_relatieaanvraagidsequence START 1;
