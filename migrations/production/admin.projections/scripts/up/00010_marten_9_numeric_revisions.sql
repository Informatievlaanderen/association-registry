ALTER TABLE public.mt_doc_bewaartermijndocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_erkenningdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_locatielookupdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_locatiezonderadresmatchdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_powerbiexportdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_powerbiexportdubbeldetectiedocument ALTER COLUMN mt_version TYPE bigint;

DROP FUNCTION IF EXISTS public.mt_get_next_hi(entity character varying) cascade;
