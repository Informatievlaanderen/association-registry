ALTER TABLE public.mt_doc_relatieaanvraagdocument DROP COLUMN mt_version;alter table public.mt_doc_relatieaanvraagdocument add column mt_version bigint NOT NULL DEFAULT 0;
