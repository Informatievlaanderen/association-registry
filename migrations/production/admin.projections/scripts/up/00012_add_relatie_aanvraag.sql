CREATE TABLE IF NOT EXISTS public.mt_doc_relatieaanvraagdocument (
    id                  varchar                     NOT NULL,
    data                jsonb                       NOT NULL,
    mt_last_modified    timestamp with time zone    NULL DEFAULT (transaction_timestamp()),
    mt_version          uuid                        NOT NULL DEFAULT (md5(random()::text || clock_timestamp()::text)::uuid),
    mt_dotnet_type      varchar                     NULL,
CONSTRAINT pkey_mt_doc_relatieaanvraagdocument_id PRIMARY KEY (id)
);

CREATE INDEX mt_doc_relatieaanvraagdocument_idx_koepel_v_code ON public.mt_doc_relatieaanvraagdocument USING btree ((data ->> 'KoepelVCode'));

CREATE INDEX mt_doc_relatieaanvraagdocument_idx_vereniging_v_code ON public.mt_doc_relatieaanvraagdocument USING btree ((data ->> 'VerenigingVCode'));
