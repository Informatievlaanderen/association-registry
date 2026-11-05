ALTER TABLE public.mt_doc_bewaartermijndocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_erkenningdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_locatielookupdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_locatiezonderadresmatchdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_powerbiexportdocument ALTER COLUMN mt_version TYPE bigint;
ALTER TABLE public.mt_doc_powerbiexportdubbeldetectiedocument ALTER COLUMN mt_version TYPE bigint;

DROP FUNCTION IF EXISTS public.mt_get_next_hi(entity character varying) cascade;

CREATE OR REPLACE FUNCTION public.mt_get_next_hi(entity varchar) RETURNS bigint AS
$$
DECLARE
    current_value bigint;
    next_value    bigint;
BEGIN
    select hi_value into current_value from public.mt_hilo where entity_name = entity;

    IF current_value is null THEN
        insert into public.mt_hilo (entity_name, hi_value) values (entity, 0);
        next_value := 0;
    ELSE
        next_value := current_value + 1;
        update public.mt_hilo set hi_value = next_value where entity_name = entity and hi_value = current_value;

        IF NOT FOUND THEN
            next_value := -1;
        END IF;
    END IF;

    return next_value;
END

$$
LANGUAGE plpgsql;
