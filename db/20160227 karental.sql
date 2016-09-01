--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2016-02-27 21:05:58

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 193 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2210 (class 0 OID 0)
-- Dependencies: 193
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 195 (class 1255 OID 25061)
-- Name: generate_rent_code(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION generate_rent_code() RETURNS text
    LANGUAGE plpgsql
    AS $$
declare
  random text := '';
  result text := '';
begin
  select random_string(5) into random;

  result = random;
  result = '123';
  
  return result;
end;
$$;


ALTER FUNCTION public.generate_rent_code() OWNER TO postgres;

--
-- TOC entry 194 (class 1255 OID 25060)
-- Name: random_string(integer); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION random_string(length integer) RETURNS text
    LANGUAGE plpgsql
    AS $$
declare
  chars text[] := '{A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z}';
  result text := '';
  i integer := 0;
begin
  if length < 0 then
    raise exception 'Given length cannot be less than 0';
  end if;
  for i in 1..length loop
    result := result || chars[1+random()*(array_length(chars, 1)-1)];
  end loop;
  return result;
end;
$$;


ALTER FUNCTION public.random_string(length integer) OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 184 (class 1259 OID 16451)
-- Name: car; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car (
    id uuid NOT NULL,
    id_car_model uuid NOT NULL,
    license_plate character varying(50) NOT NULL,
    capacity integer NOT NULL,
    status character varying(50) NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    id_owner uuid NOT NULL
);


ALTER TABLE car OWNER TO postgres;

--
-- TOC entry 185 (class 1259 OID 16454)
-- Name: car_brand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_brand (
    id uuid NOT NULL,
    name character varying(50)
);


ALTER TABLE car_brand OWNER TO postgres;

--
-- TOC entry 183 (class 1259 OID 16441)
-- Name: car_model; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_model (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    id_car_brand uuid NOT NULL
);


ALTER TABLE car_model OWNER TO postgres;

--
-- TOC entry 181 (class 1259 OID 16415)
-- Name: customer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE customer (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    customer_type character varying(50),
    phone_number character varying(100),
    address text,
    city character varying(100),
    email text,
    notes text,
    id_owner uuid NOT NULL
);


ALTER TABLE customer OWNER TO postgres;

--
-- TOC entry 182 (class 1259 OID 16428)
-- Name: driver; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE driver (
    id uuid NOT NULL,
    name text NOT NULL,
    driver_type character varying(50) NOT NULL,
    phone_number character varying(100),
    address text,
    city text,
    email text,
    notes text,
    id_owner uuid NOT NULL
);


ALTER TABLE driver OWNER TO postgres;

--
-- TOC entry 190 (class 1259 OID 25082)
-- Name: expense; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE expense (
    id uuid NOT NULL,
    id_rent uuid NOT NULL,
    date date NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone
);


ALTER TABLE expense OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 25100)
-- Name: expense_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE expense_item (
    id uuid NOT NULL,
    id_expense uuid NOT NULL,
    category character varying(50) NOT NULL,
    value integer DEFAULT 0 NOT NULL,
    description text
);


ALTER TABLE expense_item OWNER TO postgres;

--
-- TOC entry 187 (class 1259 OID 16521)
-- Name: invoice; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE invoice (
    id uuid NOT NULL,
    id_rent uuid NOT NULL,
    price integer NOT NULL,
    code character varying(50) NOT NULL,
    invoice_date date NOT NULL,
    status character varying(50) NOT NULL,
    cancel_notes text,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone
);


ALTER TABLE invoice OWNER TO postgres;

--
-- TOC entry 180 (class 1259 OID 16394)
-- Name: owner; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE owner (
    id uuid NOT NULL,
    code character varying(5) NOT NULL,
    name character varying(100) NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone,
    logo character varying(100),
    contact text,
    terms text
);


ALTER TABLE owner OWNER TO postgres;

--
-- TOC entry 188 (class 1259 OID 16801)
-- Name: owner_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE owner_user (
    id_owner uuid NOT NULL,
    username character varying(250) NOT NULL,
    id uuid NOT NULL
);


ALTER TABLE owner_user OWNER TO postgres;

--
-- TOC entry 186 (class 1259 OID 16471)
-- Name: rent; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent (
    id uuid NOT NULL,
    id_customer uuid NOT NULL,
    id_owner uuid NOT NULL,
    pickup_location text,
    id_driver uuid,
    id_car_model uuid NOT NULL,
    id_car uuid,
    start_rent timestamp with time zone NOT NULL,
    finish_rent timestamp with time zone NOT NULL,
    price integer NOT NULL,
    notes text,
    code character varying(50) NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    updated_by character varying(50),
    updated_time timestamp with time zone,
    status character varying(50),
    cancel_notes text
);


ALTER TABLE rent OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 25062)
-- Name: rent_code; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent_code (
    code character varying(10) NOT NULL
);


ALTER TABLE rent_code OWNER TO postgres;

--
-- TOC entry 192 (class 1259 OID 25114)
-- Name: view_report_driver; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW view_report_driver AS
 SELECT d.name AS driver_name,
    sum(a.value) AS amount,
    count(a.id) AS quantity
   FROM (((expense_item a
     JOIN expense b ON ((a.id_expense = b.id)))
     JOIN rent c ON ((b.id_rent = c.id)))
     JOIN driver d ON ((c.id_driver = d.id)))
  WHERE ((a.category)::text = 'DRIVER'::text)
  GROUP BY d.name;


ALTER TABLE view_report_driver OWNER TO postgres;

--
-- TOC entry 2195 (class 0 OID 16451)
-- Dependencies: 184
-- Data for Name: car; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car VALUES ('bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '125700cd-f447-4a6c-9e19-206c36a8369e', 'I 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('892b1090-d859-4a06-8922-932bdf7e9d40', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('4b96c548-f6b3-420a-8a20-000388bc0bb4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1122 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO car VALUES ('4ac204b3-fc1d-4cc3-bd46-132d2e3c8ca7', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 2233 BB', 6, 'AVAILABLE', true, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');


--
-- TOC entry 2196 (class 0 OID 16454)
-- Dependencies: 185
-- Data for Name: car_brand; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_brand VALUES ('eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 'Toyota');
INSERT INTO car_brand VALUES ('9572d6ec-a2df-4407-b2f8-f06d61dd122f', 'Honda');


--
-- TOC entry 2194 (class 0 OID 16441)
-- Dependencies: 183
-- Data for Name: car_model; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_model VALUES ('545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'Avanza', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576');
INSERT INTO car_model VALUES ('125700cd-f447-4a6c-9e19-206c36a8369e', 'Innova', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576');
INSERT INTO car_model VALUES ('913e8705-f367-4e00-a6a5-6b403156c79b', 'Freed', '9572d6ec-a2df-4407-b2f8-f06d61dd122f');


--
-- TOC entry 2192 (class 0 OID 16415)
-- Dependencies: 181
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO customer VALUES ('46d30608-492e-44a1-9cfe-37b5584ed4c1', 'Hutami', 'Corporate', '08112210960', NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');
INSERT INTO customer VALUES ('d3e5b635-7c36-47bd-b215-41896ccc523f', 'Bolu', NULL, NULL, 'Sawah Kurung', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('199d978b-3eb1-4598-9613-84928b05f9fc', 'Lauw Siu Hung', 'CORPORATE', '08122356566', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('e9b9d948-0a77-4089-aca0-324b913d0d34', 'Chandra', 'CORPORATE', '085722925900', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('e19b1f80-a675-42e0-a707-9da8c99730b5', 'Ronny', NULL, '081111113', 'Alamat 2', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('e5e5491b-e655-4dd9-a78e-c990e820670a', 'Michael James', NULL, '081111114', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO customer VALUES ('4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'Andreas', NULL, '081111112', 'Alamat', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');


--
-- TOC entry 2193 (class 0 OID 16428)
-- Dependencies: 182
-- Data for Name: driver; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO driver VALUES ('ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', 'Suryono', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO driver VALUES ('807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', 'Johnny', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9');
INSERT INTO driver VALUES ('1d4a3bfe-a585-42e8-9b67-17f4309108e6', 'Bobon', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5');


--
-- TOC entry 2201 (class 0 OID 25082)
-- Dependencies: 190
-- Data for Name: expense; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO expense VALUES ('b3cb5043-26ea-4966-9385-6b8cb9ee42e3', '06c81690-1622-4b3e-a53e-689e4c6b5cd6', '2016-02-27', 'reynhart', '2016-02-27 00:00:00+00', NULL, NULL);
INSERT INTO expense VALUES ('28f3219c-2fac-4a30-bb5f-f3c7f8612e41', '67d1b0f6-fa3d-4546-8f77-be893ae2291c', '2016-02-27', 'reynhart', '2016-02-27 00:00:00+00', NULL, NULL);
INSERT INTO expense VALUES ('499c79dc-70d2-4f67-a024-29f4d60064d1', '60e8a355-702a-4c76-b8c0-942f6e413896', '2016-02-27', 'reynhart', '2016-02-26 00:00:00+00', NULL, NULL);


--
-- TOC entry 2202 (class 0 OID 25100)
-- Dependencies: 191
-- Data for Name: expense_item; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO expense_item VALUES ('6cc4b536-efe7-4d2d-a0c4-a8e2b0c5f6f1', 'b3cb5043-26ea-4966-9385-6b8cb9ee42e3', 'DRIVER', 100000, NULL);
INSERT INTO expense_item VALUES ('7fdd3a13-d5ad-4688-958a-03b82626d43a', '28f3219c-2fac-4a30-bb5f-f3c7f8612e41', 'DRIVER', 200000, NULL);
INSERT INTO expense_item VALUES ('828596e1-c0b5-45be-a908-0792c7862e35', '499c79dc-70d2-4f67-a024-29f4d60064d1', 'DRIVER', 150000, NULL);
INSERT INTO expense_item VALUES ('87ea003a-1bfa-40f9-b309-432ca41cf824', '499c79dc-70d2-4f67-a024-29f4d60064d1', 'VEHICLE', 300000, NULL);


--
-- TOC entry 2198 (class 0 OID 16521)
-- Dependencies: 187
-- Data for Name: invoice; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO invoice VALUES ('885e527e-8b3d-4c70-aa23-9c36ba9c32c3', 'a38ec800-ee61-4cc9-9e98-b4378a6e90ce', 100000, '123', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 04:41:29.020277+00', NULL, NULL);
INSERT INTO invoice VALUES ('7d3e7653-e7c6-472c-8bc8-7fa62a3453b9', '92b4b48d-37e1-4342-b1d7-96ab0ee5fc95', 200000, '111', '2016-01-04', 'PAID', NULL, 'reynhart', '2016-01-03 04:41:54.43073+00', NULL, NULL);
INSERT INTO invoice VALUES ('ce4e6780-21d3-4865-a39e-c108e82c0996', '64fb50fd-3090-44ae-b981-4c00e9633d01', 150000, '112', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 04:42:13.214805+00', NULL, NULL);
INSERT INTO invoice VALUES ('3eaaadac-6677-404f-a818-accdd2a8ed22', '68d405d4-cea6-41cb-b4a1-4e7ab4b9f0a6', 1233, 'INV-0131-B', '2016-02-02', 'PAID', NULL, 'reynhart', '2016-01-31 05:20:32.63084+00', 'reynhart', '2016-01-31 05:21:50.977322+00');


--
-- TOC entry 2191 (class 0 OID 16394)
-- Dependencies: 180
-- Data for Name: owner; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'RK', 'Rekadia', 'superadmin', '2014-12-31 17:00:00+00', NULL, NULL, NULL, NULL, NULL);
INSERT INTO owner VALUES ('f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'AM', 'Ameco', 'superadmin', '2015-02-01 17:00:00+00', NULL, NULL, NULL, NULL, NULL);


--
-- TOC entry 2199 (class 0 OID 16801)
-- Dependencies: 188
-- Data for Name: owner_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'reynhart', 'e59fc4a1-11b7-44ee-9c75-73a645693269');


--
-- TOC entry 2197 (class 0 OID 16471)
-- Dependencies: 186
-- Data for Name: rent; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO rent VALUES ('60e8a355-702a-4c76-b8c0-942f6e413896', '199d978b-3eb1-4598-9613-84928b05f9fc', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2015-12-31 08:00:00+00', '2016-01-01 08:00:00+00', 300000, 'catatan<br />123', 'RK/FH145', 'reynhart', '2015-12-31 07:49:41.641809+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('148fb2d5-c372-4bab-a313-423dfa838b32', '46d30608-492e-44a1-9cfe-37b5584ed4c1', 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'Kosambi', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2015-12-31 17:00:00+00', '2016-02-01 17:00:00+00', 300000, NULL, 'AM/ABCD', 'reynhart', '2015-12-30 17:00:00+00', NULL, NULL, NULL, NULL);
INSERT INTO rent VALUES ('5f4c62a0-58ea-444a-8623-00b72fc74563', 'e19b1f80-a675-42e0-a707-9da8c99730b5', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Penjemputan 2', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-01-01 09:00:00+00', '2016-01-02 09:00:00+00', 200000, 'catat<br />catat<br />catat', 'RK/156WW', 'reynhart', '2015-12-31 08:21:12.417955+00', 'reynhart', '2016-01-02 09:52:43.28185+00', 'GO', NULL);
INSERT INTO rent VALUES ('92b4b48d-37e1-4342-b1d7-96ab0ee5fc95', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-01 04:00:00+00', '2016-01-03 04:00:00+00', 250000, NULL, 'RK/PQ7A5', 'reynhart', '2016-01-02 03:33:02.262059+00', 'reynhart', '2016-01-02 09:53:07.226219+00', 'NEW', NULL);
INSERT INTO rent VALUES ('67d1b0f6-fa3d-4546-8f77-be893ae2291c', '199d978b-3eb1-4598-9613-84928b05f9fc', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84 1', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2016-01-02 08:00:00+00', '2016-01-03 08:00:00+00', 400000, NULL, 'RK/8D3QQ', 'reynhart', '2015-12-31 07:53:43.529644+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('a38ec800-ee61-4cc9-9e98-b4378a6e90ce', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Buah Batu', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-01 20:30:00+00', '2016-01-02 02:00:00+00', 150000, NULL, 'RK/4N00Q', 'reynhart', '2016-01-02 03:23:35.053617+00', 'reynhart', '2016-01-03 04:50:39.989791+00', 'CANCEL', 'tidak jadi');
INSERT INTO rent VALUES ('4683a7ed-44d8-4350-9729-298dea3ffaf9', 'e19b1f80-a675-42e0-a707-9da8c99730b5', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Medit 2', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-22 00:00:00+00', '2016-01-23 00:00:00+00', 123123, NULL, 'RK/2DVGZ', 'reynhart', '2016-01-20 23:19:25.826273+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('be0aa1e3-659a-40d2-806a-fff3701e62d6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Medit 1', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-20 23:00:00+00', '2016-01-21 20:00:00+00', 123123, NULL, 'RK/VKD04', 'reynhart', '2016-01-20 23:17:25.551393+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('68d405d4-cea6-41cb-b4a1-4e7ab4b9f0a6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-29 15:00:00+00', '2016-01-30 15:00:00+00', 123, NULL, 'RK/0FU50', 'reynhart', '2016-01-29 14:41:41.900242+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('06c81690-1622-4b3e-a53e-689e4c6b5cd6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-01-03 12:00:00+00', '2016-01-04 12:00:00+00', 300000, NULL, 'RK/YN32U', 'reynhart', '2016-01-03 11:25:17.407034+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('cedd612d-0fcc-45b0-b749-1326f6345ba8', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', '123', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-01-29 15:00:00+00', '2016-01-30 15:00:00+00', 123, NULL, 'RK/P7X33', 'reynhart', '2016-01-29 14:43:08.187177+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('64fb50fd-3090-44ae-b981-4c00e9633d01', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'TSM', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-04 04:00:00+00', '2016-01-09 04:00:00+00', 350000, 'tes<br />tes', 'RK/9SZY0', 'reynhart', '2016-01-02 03:02:17.94857+00', 'reynhart', '2016-01-06 18:12:11.635528+00', 'CANCEL', 'tes<br />tes');
INSERT INTO rent VALUES ('ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-02-02 06:00:00+00', '2016-02-03 06:00:00+00', 123, NULL, 'RK/XIIFM', 'reynhart', '2016-02-02 05:36:47.720562+00', 'reynhart', '2016-02-17 14:56:40.233443+00', 'FINISH', NULL);
INSERT INTO rent VALUES ('693eec73-0db3-45dc-b0c8-50d003fd4370', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-03 12:00:00+00', '2016-01-04 12:00:00+00', 300000, NULL, 'RK/0O9P1', 'reynhart', '2016-01-03 11:28:55.775524+00', 'reynhart', '2016-01-03 11:31:30.366366+00', 'NEW', NULL);


--
-- TOC entry 2200 (class 0 OID 25062)
-- Dependencies: 189
-- Data for Name: rent_code; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO rent_code VALUES ('RK/NXIG2');
INSERT INTO rent_code VALUES ('RK/FH145');
INSERT INTO rent_code VALUES ('RK/8D3QQ');
INSERT INTO rent_code VALUES ('RK/156WW');
INSERT INTO rent_code VALUES ('RK/9SZY0');
INSERT INTO rent_code VALUES ('RK/4N00Q');
INSERT INTO rent_code VALUES ('RK/7NURJ');
INSERT INTO rent_code VALUES ('RK/PQ7A5');
INSERT INTO rent_code VALUES ('RK/YN32U');
INSERT INTO rent_code VALUES ('RK/0O9P1');
INSERT INTO rent_code VALUES ('RK/VKD04');
INSERT INTO rent_code VALUES ('RK/2DVGZ');
INSERT INTO rent_code VALUES ('RK/0FU50');
INSERT INTO rent_code VALUES ('RK/P7X33');
INSERT INTO rent_code VALUES ('RK/XIIFM');


--
-- TOC entry 2039 (class 2606 OID 16402)
-- Name: Primary Key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner
    ADD CONSTRAINT "Primary Key" PRIMARY KEY (id);


--
-- TOC entry 2047 (class 2606 OID 16465)
-- Name: pk_car; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT pk_car PRIMARY KEY (id);


--
-- TOC entry 2049 (class 2606 OID 16458)
-- Name: pk_car_brand; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_brand
    ADD CONSTRAINT pk_car_brand PRIMARY KEY (id);


--
-- TOC entry 2045 (class 2606 OID 16445)
-- Name: pk_car_type; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT pk_car_type PRIMARY KEY (id);


--
-- TOC entry 2059 (class 2606 OID 25086)
-- Name: pk_cost; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT pk_cost PRIMARY KEY (id);


--
-- TOC entry 2061 (class 2606 OID 25108)
-- Name: pk_cost_item; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT pk_cost_item PRIMARY KEY (id);


--
-- TOC entry 2041 (class 2606 OID 16422)
-- Name: pk_customer; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT pk_customer PRIMARY KEY (id);


--
-- TOC entry 2043 (class 2606 OID 16435)
-- Name: pk_driver; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT pk_driver PRIMARY KEY (id);


--
-- TOC entry 2053 (class 2606 OID 16529)
-- Name: pk_invoice; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT pk_invoice PRIMARY KEY (id);


--
-- TOC entry 2055 (class 2606 OID 16808)
-- Name: pk_owner_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT pk_owner_user PRIMARY KEY (id);


--
-- TOC entry 2051 (class 2606 OID 16475)
-- Name: pk_rent; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT pk_rent PRIMARY KEY (id);


--
-- TOC entry 2057 (class 2606 OID 25066)
-- Name: pk_rent_code; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_code
    ADD CONSTRAINT pk_rent_code PRIMARY KEY (code);


--
-- TOC entry 2065 (class 2606 OID 16466)
-- Name: fk_car_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2064 (class 2606 OID 16459)
-- Name: fk_car_model_car_brand; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT fk_car_model_car_brand FOREIGN KEY (id_car_brand) REFERENCES car_brand(id);


--
-- TOC entry 2066 (class 2606 OID 16868)
-- Name: fk_car_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2075 (class 2606 OID 25109)
-- Name: fk_cost_item_cost; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT fk_cost_item_cost FOREIGN KEY (id_expense) REFERENCES expense(id);


--
-- TOC entry 2074 (class 2606 OID 25087)
-- Name: fk_cost_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT fk_cost_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2062 (class 2606 OID 16423)
-- Name: fk_customer_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT fk_customer_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2063 (class 2606 OID 16436)
-- Name: fk_driver_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT fk_driver_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2072 (class 2606 OID 16530)
-- Name: fk_invoice_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT fk_invoice_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2073 (class 2606 OID 16809)
-- Name: fk_ou_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT fk_ou_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2071 (class 2606 OID 16499)
-- Name: fk_rent_car; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car FOREIGN KEY (id_car) REFERENCES car(id);


--
-- TOC entry 2070 (class 2606 OID 16494)
-- Name: fk_rent_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2067 (class 2606 OID 16476)
-- Name: fk_rent_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_customer FOREIGN KEY (id_customer) REFERENCES customer(id);


--
-- TOC entry 2069 (class 2606 OID 16489)
-- Name: fk_rent_driver; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_driver FOREIGN KEY (id_driver) REFERENCES driver(id);


--
-- TOC entry 2068 (class 2606 OID 16484)
-- Name: fk_rent_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2209 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2016-02-27 21:05:59

--
-- PostgreSQL database dump complete
--

