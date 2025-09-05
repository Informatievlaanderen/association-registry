DO LANGUAGE plpgsql $tran$
BEGIN

DO $$
BEGIN    IF NOT EXISTS(
        SELECT schema_name
          FROM information_schema.schemata
          WHERE schema_name = 'public'
      )
    THEN
      EXECUTE 'CREATE SCHEMA public';
    END IF;

END
$$;


DROP TABLE IF EXISTS public.wolverine_outgoing_envelopes CASCADE;
CREATE TABLE public.wolverine_outgoing_envelopes (
    id              uuid                        NOT NULL,
    owner_id        integer                     NOT NULL,
    destination     varchar                     NOT NULL,
    deliver_by      timestamp with time zone    NULL,
    body            bytea                       NOT NULL,
    attempts        integer                     NULL DEFAULT 0,
    message_type    varchar                     NOT NULL,
CONSTRAINT pkey_wolverine_outgoing_envelopes_id PRIMARY KEY (id)
);
DROP TABLE IF EXISTS public.wolverine_incoming_envelopes CASCADE;
CREATE TABLE public.wolverine_incoming_envelopes (
    id                uuid                        NOT NULL,
    status            varchar                     NOT NULL,
    owner_id          integer                     NOT NULL,
    execution_time    timestamp with time zone    NULL DEFAULT NULL,
    attempts          integer                     NULL DEFAULT 0,
    body              bytea                       NOT NULL,
    message_type      varchar                     NOT NULL,
    received_at       varchar                     NULL,
    keep_until        timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_incoming_envelopes_id PRIMARY KEY (id)
);
DROP TABLE IF EXISTS public.wolverine_dead_letters CASCADE;
CREATE TABLE public.wolverine_dead_letters (
    id                   uuid                        NOT NULL,
    execution_time       timestamp with time zone    NULL DEFAULT NULL,
    body                 bytea                       NOT NULL,
    message_type         varchar                     NOT NULL,
    received_at          varchar                     NULL,
    source               varchar                     NULL,
    exception_type       varchar                     NULL,
    exception_message    varchar                     NULL,
    sent_at              timestamp with time zone    NULL,
    replayable           boolean                     NULL,
CONSTRAINT pkey_wolverine_dead_letters_id PRIMARY KEY (id)
);
DROP TABLE IF EXISTS public.wolverine_nodes CASCADE;
CREATE TABLE public.wolverine_nodes (
    id              uuid                        NOT NULL,
    node_number     serial                      NOT NULL,
    description     varchar                     NOT NULL,
    uri             varchar                     NOT NULL,
    started         timestamp with time zone    NOT NULL DEFAULT now(),
    health_check    timestamp with time zone    NOT NULL DEFAULT now(),
    version         varchar                     NULL,
    capabilities    text[]                      NULL,
CONSTRAINT pkey_wolverine_nodes_id PRIMARY KEY (id)
);
DROP TABLE IF EXISTS public.wolverine_node_assignments CASCADE;
CREATE TABLE public.wolverine_node_assignments (
    id         varchar                     NOT NULL,
    node_id    uuid                        NULL,
    started    timestamp with time zone    NOT NULL DEFAULT now(),
CONSTRAINT pkey_wolverine_node_assignments_id PRIMARY KEY (id)
);

ALTER TABLE public.wolverine_node_assignments
ADD CONSTRAINT fkey_wolverine_node_assignments_node_id FOREIGN KEY(node_id)
REFERENCES public.wolverine_nodes(id)ON DELETE CASCADE
;

DROP TABLE IF EXISTS public.wolverine_control_queue CASCADE;
CREATE TABLE public.wolverine_control_queue (
    id              uuid                        NOT NULL,
    message_type    varchar                     NOT NULL,
    node_id         uuid                        NOT NULL,
    body            bytea                       NOT NULL,
    posted          timestamp with time zone    NOT NULL DEFAULT NOW(),
    expires         timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_control_queue_id PRIMARY KEY (id)
);
DROP TABLE IF EXISTS public.wolverine_node_records CASCADE;
CREATE TABLE public.wolverine_node_records (
    id             serial                      NOT NULL,
    node_number    integer                     NOT NULL,
    event_name     varchar                     NOT NULL,
    timestamp      timestamp with time zone    NOT NULL DEFAULT now(),
    description    varchar                     NULL,
CONSTRAINT pkey_wolverine_node_records_id PRIMARY KEY (id)
);

END;
$tran$;
