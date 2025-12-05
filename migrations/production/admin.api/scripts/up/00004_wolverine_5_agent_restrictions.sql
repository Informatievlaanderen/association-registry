DO LANGUAGE plpgsql $tran$
BEGIN

DROP TABLE IF EXISTS public.wolverine_agent_restrictions CASCADE;
CREATE TABLE public.wolverine_agent_restrictions (
    id      uuid       NOT NULL,
    uri     varchar    NOT NULL,
    type    varchar    NOT NULL,
    node    integer    NOT NULL DEFAULT 0,
CONSTRAINT pkey_wolverine_agent_restrictions_id PRIMARY KEY (id)
);

END;
$tran$;
