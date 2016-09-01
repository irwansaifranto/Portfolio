--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5.1
-- Dumped by pg_dump version 9.5.1

-- Started on 2016-05-24 10:27:18

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 1 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2248 (class 0 OID 0)
-- Dependencies: 1
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 199 (class 1255 OID 16396)
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
-- TOC entry 200 (class 1255 OID 16397)
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
-- TOC entry 181 (class 1259 OID 16398)
-- Name: car; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car (
    id uuid NOT NULL,
    id_car_model uuid NOT NULL,
    license_plate character varying(50) NOT NULL,
    capacity integer,
    status character varying(50),
    is_active boolean DEFAULT true NOT NULL,
    id_owner uuid NOT NULL,
    model_year integer,
    transmission character varying(4),
    fuel character varying(8),
    color character varying(16),
    photo character varying(128)
);


ALTER TABLE car OWNER TO postgres;

--
-- TOC entry 182 (class 1259 OID 16402)
-- Name: car_brand; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_brand (
    id uuid NOT NULL,
    name character varying(50)
);


ALTER TABLE car_brand OWNER TO postgres;

--
-- TOC entry 183 (class 1259 OID 16405)
-- Name: car_model; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE car_model (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    id_car_brand uuid NOT NULL,
    capacity smallint DEFAULT 0 NOT NULL
);


ALTER TABLE car_model OWNER TO postgres;

--
-- TOC entry 198 (class 1259 OID 33286)
-- Name: city_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE city_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE city_seq OWNER TO postgres;

--
-- TOC entry 197 (class 1259 OID 33281)
-- Name: city; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE city (
    id integer DEFAULT nextval('city_seq'::regclass) NOT NULL,
    name character varying(50) NOT NULL,
    latitude double precision NOT NULL,
    longitude double precision NOT NULL
);


ALTER TABLE city OWNER TO postgres;

--
-- TOC entry 184 (class 1259 OID 16409)
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
    id_owner uuid NOT NULL,
    title character varying(4),
    company character varying(32),
    photo character varying(128)
);


ALTER TABLE customer OWNER TO postgres;

--
-- TOC entry 185 (class 1259 OID 16415)
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
    id_owner uuid NOT NULL,
    work_start_date date,
    photo character varying(128),
    username character varying(50),
    device_id character varying(50)
);


ALTER TABLE driver OWNER TO postgres;

--
-- TOC entry 186 (class 1259 OID 16421)
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
-- TOC entry 187 (class 1259 OID 16424)
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
-- TOC entry 188 (class 1259 OID 16431)
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
    updated_time timestamp with time zone,
    ppn boolean DEFAULT false NOT NULL,
    total integer DEFAULT 0 NOT NULL
);


ALTER TABLE invoice OWNER TO postgres;

--
-- TOC entry 189 (class 1259 OID 16439)
-- Name: invoice_item; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE invoice_item (
    id uuid NOT NULL,
    id_invoice uuid NOT NULL,
    category character varying(255) NOT NULL,
    value integer NOT NULL
);


ALTER TABLE invoice_item OWNER TO postgres;

--
-- TOC entry 196 (class 1259 OID 33261)
-- Name: log_ws; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE log_ws (
    created_time timestamp with time zone NOT NULL,
    request_body text NOT NULL,
    response_body text,
    url character varying(50) NOT NULL,
    request_header text,
    response_header text,
    id uuid NOT NULL
);


ALTER TABLE log_ws OWNER TO postgres;

--
-- TOC entry 190 (class 1259 OID 16442)
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
    terms text,
    id_city integer NOT NULL
);


ALTER TABLE owner OWNER TO postgres;

--
-- TOC entry 191 (class 1259 OID 16448)
-- Name: owner_user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE owner_user (
    id_owner uuid NOT NULL,
    username character varying(250) NOT NULL,
    id uuid NOT NULL
);


ALTER TABLE owner_user OWNER TO postgres;

--
-- TOC entry 192 (class 1259 OID 16451)
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
-- TOC entry 193 (class 1259 OID 16457)
-- Name: rent_code; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent_code (
    code character varying(10) NOT NULL
);


ALTER TABLE rent_code OWNER TO postgres;

--
-- TOC entry 195 (class 1259 OID 33251)
-- Name: rent_position; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE rent_position (
    id uuid NOT NULL,
    id_rent uuid NOT NULL,
    created_by character varying(50) NOT NULL,
    created_time timestamp with time zone NOT NULL,
    latitude double precision NOT NULL,
    longitude double precision NOT NULL
);


ALTER TABLE rent_position OWNER TO postgres;

--
-- TOC entry 194 (class 1259 OID 16460)
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
-- TOC entry 2224 (class 0 OID 16398)
-- Dependencies: 181
-- Data for Name: car; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car VALUES ('bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '125700cd-f447-4a6c-9e19-206c36a8369e', 'I 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL, NULL, NULL);
INSERT INTO car VALUES ('892b1090-d859-4a06-8922-932bdf7e9d40', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1234 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL, NULL, NULL);
INSERT INTO car VALUES ('4b96c548-f6b3-420a-8a20-000388bc0bb4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 1122 AA', 6, 'AVAILABLE', true, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL, NULL, NULL);
INSERT INTO car VALUES ('4ac204b3-fc1d-4cc3-bd46-132d2e3c8ca7', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'A 2233 BB', 6, 'AVAILABLE', true, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', NULL, NULL, NULL, NULL, NULL);
INSERT INTO car VALUES ('716bfc3f-054f-49dd-b741-069e7e080b27', '913e8705-f367-4e00-a6a5-6b403156c79b', 'DK 1111 AA', NULL, NULL, true, '157701c3-5f4f-4626-96bd-a905ababd469', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('0d00ba0a-dec3-4139-ac51-66125a3e56c4', '913e8705-f367-4e00-a6a5-6b403156c79b', 'DK 1112 AB', NULL, NULL, true, '157701c3-5f4f-4626-96bd-a905ababd469', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('56a9f8a0-71bf-45a3-9905-92803fe5a851', '3946280e-8a82-41ee-80cd-748841e175b6', 'DK 2221 AB', NULL, NULL, true, '157701c3-5f4f-4626-96bd-a905ababd469', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('6d3aca90-42f9-4324-bc7b-7991f553d11d', '3946280e-8a82-41ee-80cd-748841e175b6', 'DK 2222 AB', NULL, NULL, true, '157701c3-5f4f-4626-96bd-a905ababd469', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('222d3559-94ad-4f4e-b50f-6b0cd4debf56', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'D 3331 AA', NULL, NULL, true, '60c59a63-027f-484b-b8c4-4a03d2cd9b80', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('1a7a7cfc-3cc0-428d-be9a-5310898da0bf', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'D 3332 AA', NULL, NULL, false, '60c59a63-027f-484b-b8c4-4a03d2cd9b80', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('7cdf4adf-5c83-4183-bf5f-4dbfd5c7ba88', '125700cd-f447-4a6c-9e19-206c36a8369e', 'D 4441 AA', NULL, NULL, false, '60c59a63-027f-484b-b8c4-4a03d2cd9b80', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('27f9fa87-ca01-4c6d-b39d-cad6922cc8bd', '125700cd-f447-4a6c-9e19-206c36a8369e', 'D 4442 AA', NULL, NULL, true, '60c59a63-027f-484b-b8c4-4a03d2cd9b80', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('7c264527-2ab4-4486-8ecf-402cd3ad8dba', '913e8705-f367-4e00-a6a5-6b403156c79b', 'DD 1234 CDE', NULL, NULL, false, 'd021a28b-6aca-4bbf-930c-935ff63691e9', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('3b643150-ecb8-41e1-a4cd-2d689ba28c77', '125700cd-f447-4a6c-9e19-206c36a8369e', 'A  234  DSD', NULL, NULL, false, '157701c3-5f4f-4626-96bd-a905ababd469', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('4ed84ab4-33ea-4def-8887-2dc14b8759f6', '913e8705-f367-4e00-a6a5-6b403156c79b', 'LK 8798 JUH', NULL, NULL, false, 'd021a28b-6aca-4bbf-930c-935ff63691e9', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('67bb1fd9-92fd-47fa-a4b1-4524bdaec426', '125700cd-f447-4a6c-9e19-206c36a8369e', 'D  1234 ASD', NULL, NULL, true, '9e9af49f-742a-40a2-9338-228e56060442', 2016, 'AT', 'GASOLINE', NULL, NULL);
INSERT INTO car VALUES ('6f6e04a4-49a6-440f-b4dd-916e156b114b', '913e8705-f367-4e00-a6a5-6b403156c79b', 'D  1234 ZXC', NULL, NULL, true, '7a683eb8-656d-4b4c-be71-157dd2328a64', 2016, 'AT', 'GASOLINE', NULL, NULL);


--
-- TOC entry 2225 (class 0 OID 16402)
-- Dependencies: 182
-- Data for Name: car_brand; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_brand VALUES ('eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 'Toyota');
INSERT INTO car_brand VALUES ('9572d6ec-a2df-4407-b2f8-f06d61dd122f', 'Honda');


--
-- TOC entry 2226 (class 0 OID 16405)
-- Dependencies: 183
-- Data for Name: car_model; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_model VALUES ('545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'Avanza', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 0);
INSERT INTO car_model VALUES ('125700cd-f447-4a6c-9e19-206c36a8369e', 'Innova', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 0);
INSERT INTO car_model VALUES ('3946280e-8a82-41ee-80cd-748841e175b6', 'Jazz', '9572d6ec-a2df-4407-b2f8-f06d61dd122f', 3);
INSERT INTO car_model VALUES ('913e8705-f367-4e00-a6a5-6b403156c79b', 'Freed', '9572d6ec-a2df-4407-b2f8-f06d61dd122f', 5);


--
-- TOC entry 2239 (class 0 OID 33281)
-- Dependencies: 197
-- Data for Name: city; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO city VALUES (1, 'Bandung', -6.9216340000000001, 107.611237);


--
-- TOC entry 2249 (class 0 OID 0)
-- Dependencies: 198
-- Name: city_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('city_seq', 1, true);


--
-- TOC entry 2227 (class 0 OID 16409)
-- Dependencies: 184
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO customer VALUES ('46d30608-492e-44a1-9cfe-37b5584ed4c1', 'Hutami', 'Corporate', '08112210960', NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', NULL, NULL, NULL);
INSERT INTO customer VALUES ('d3e5b635-7c36-47bd-b215-41896ccc523f', 'Bolu', NULL, NULL, 'Sawah Kurung', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('e19b1f80-a675-42e0-a707-9da8c99730b5', 'Ronny', NULL, '081111113', 'Alamat 2', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'Dwijayanti', 'PERSONAL', '081111115;081111115___', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'Andreas', 'PERSONAL', '081111112;082222222___', 'Alamat', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('e9b9d948-0a77-4089-aca0-324b913d0d34', 'Chandra', 'CORPORATE', '085722925900;085722925900', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('3186759c-b0d8-432b-b76d-ef4b1dfc91ef', 'bright', NULL, '123', NULL, NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('32892c3c-a935-4599-b251-2102caf4ff35', 'bright', NULL, '123', NULL, NULL, NULL, NULL, '9e9af49f-742a-40a2-9338-228e56060442', NULL, NULL, NULL);
INSERT INTO customer VALUES ('57e04472-e643-44f7-aa69-48cabf3a603c', 'asdf', 'PERSONAL', '123         ;', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'MR', NULL, NULL);
INSERT INTO customer VALUES ('e5e5491b-e655-4dd9-a78e-c990e820670a', 'Michael James', NULL, '081111114;081111114___', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('199d978b-3eb1-4598-9613-84928b05f9fc', 'Lauw Siu Hung', 'CORPORATE', '08122356566', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('f7a3cd21-ce18-4683-a5fd-906c1859fd30', 'joni', NULL, '021458796554;', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('a0017e50-3e29-4095-8b83-cf0cfe142243', 'Kurnia', NULL, '081111116', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('854dca36-71eb-4dd1-a4ea-d627e6857eb5', 'Chandra', NULL, '1111;123123______', 'Jalan Veteran 123', NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('283cc7ff-c4e6-4ffb-aaf5-186d71e61566', 'Yosef', NULL, '1112;222_________', 'sawah 1', NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('107aa33b-07ad-4c3b-96ef-4fe2760b295f', 'Bolu', NULL, '111;222_________', 'SSSS', NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('0a54ef48-eccf-46c8-b503-7dfc1de2f70d', 'Bimo', NULL, '999;888_________', 'ddd', NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('33717cf2-ea31-4849-8aa1-a53c6806de61', 'Andre', NULL, '111;', 'SSSS', NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);
INSERT INTO customer VALUES ('a8ea9dc1-4db7-4cc8-a0e3-6dada4ad3f5f', 'Nala', NULL, '123;345_________', NULL, NULL, NULL, NULL, '157701c3-5f4f-4626-96bd-a905ababd469', NULL, NULL, NULL);


--
-- TOC entry 2228 (class 0 OID 16415)
-- Dependencies: 185
-- Data for Name: driver; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO driver VALUES ('1d4a3bfe-a585-42e8-9b67-17f4309108e6', 'Bobon', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', NULL, NULL, NULL, NULL);
INSERT INTO driver VALUES ('9965901e-8618-43b3-a5fd-5a4297ebab37', 'Yono', 'PERSONAL', '+6285722925900', 'Veteran 84', 'Bandung', 'chandra.oey@gmail.com', NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, 'yono', NULL);
INSERT INTO driver VALUES ('807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', 'Johnny', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, 'reynhart', 'android');
INSERT INTO driver VALUES ('ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', 'Suryono', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, 'A10', 'android');


--
-- TOC entry 2229 (class 0 OID 16421)
-- Dependencies: 186
-- Data for Name: expense; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO expense VALUES ('b3cb5043-26ea-4966-9385-6b8cb9ee42e3', '06c81690-1622-4b3e-a53e-689e4c6b5cd6', '2016-02-27', 'reynhart', '2016-02-27 07:00:00+07', NULL, NULL);
INSERT INTO expense VALUES ('28f3219c-2fac-4a30-bb5f-f3c7f8612e41', '67d1b0f6-fa3d-4546-8f77-be893ae2291c', '2016-02-27', 'reynhart', '2016-02-27 07:00:00+07', NULL, NULL);
INSERT INTO expense VALUES ('499c79dc-70d2-4f67-a024-29f4d60064d1', '60e8a355-702a-4c76-b8c0-942f6e413896', '2016-02-27', 'reynhart', '2016-02-26 07:00:00+07', NULL, NULL);
INSERT INTO expense VALUES ('12e0dddc-2a98-4500-a514-78dec3856c5a', 'efe7eb14-269e-4863-9fd3-a63cdc792f0b', '2016-03-14', 'reynhart', '2016-03-14 12:20:31.175178+07', 'reynhart', '2016-03-15 17:10:34.676988+07');


--
-- TOC entry 2230 (class 0 OID 16424)
-- Dependencies: 187
-- Data for Name: expense_item; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO expense_item VALUES ('6cc4b536-efe7-4d2d-a0c4-a8e2b0c5f6f1', 'b3cb5043-26ea-4966-9385-6b8cb9ee42e3', 'DRIVER', 100000, NULL);
INSERT INTO expense_item VALUES ('7fdd3a13-d5ad-4688-958a-03b82626d43a', '28f3219c-2fac-4a30-bb5f-f3c7f8612e41', 'DRIVER', 200000, NULL);
INSERT INTO expense_item VALUES ('828596e1-c0b5-45be-a908-0792c7862e35', '499c79dc-70d2-4f67-a024-29f4d60064d1', 'DRIVER', 150000, NULL);
INSERT INTO expense_item VALUES ('87ea003a-1bfa-40f9-b309-432ca41cf824', '499c79dc-70d2-4f67-a024-29f4d60064d1', 'VEHICLE', 300000, NULL);
INSERT INTO expense_item VALUES ('41087fd9-e95b-4949-aa59-86c0f1312f6a', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'VEHICLE', 300000, 'Pungutan liar');
INSERT INTO expense_item VALUES ('806c1938-49ef-4337-8796-87bc33ec3184', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'DRIVER', 100000, 'Pungutan liar');
INSERT INTO expense_item VALUES ('01e8b885-c9db-479d-8bfc-baa77254bde2', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'GAS', 100000, 'Pungutan liar');
INSERT INTO expense_item VALUES ('2c8464fb-8950-4a79-9dbb-264b75496e16', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'TOLL', 50000, 'Pungutan liar');
INSERT INTO expense_item VALUES ('ab5e8d9c-048a-4d4a-9d7c-3e40c795796d', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'PARKING', 5000, 'Pungutan liar');
INSERT INTO expense_item VALUES ('4bf79541-90a4-40f1-b5c2-52559d981135', '12e0dddc-2a98-4500-a514-78dec3856c5a', 'OTHER', 10000, 'Pungutan liar');


--
-- TOC entry 2231 (class 0 OID 16431)
-- Dependencies: 188
-- Data for Name: invoice; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO invoice VALUES ('885e527e-8b3d-4c70-aa23-9c36ba9c32c3', 'a38ec800-ee61-4cc9-9e98-b4378a6e90ce', 100000, '123', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 11:41:29.020277+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('7d3e7653-e7c6-472c-8bc8-7fa62a3453b9', '92b4b48d-37e1-4342-b1d7-96ab0ee5fc95', 200000, '111', '2016-01-04', 'PAID', NULL, 'reynhart', '2016-01-03 11:41:54.43073+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('ce4e6780-21d3-4865-a39e-c108e82c0996', '64fb50fd-3090-44ae-b981-4c00e9633d01', 150000, '112', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 11:42:13.214805+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('3eaaadac-6677-404f-a818-accdd2a8ed22', '68d405d4-cea6-41cb-b4a1-4e7ab4b9f0a6', 1233, 'INV-0131-B', '2016-02-02', 'PAID', NULL, 'reynhart', '2016-01-31 12:20:32.63084+07', 'reynhart', '2016-01-31 12:21:50.977322+07', false, 0);
INSERT INTO invoice VALUES ('c60e29ad-d5da-4e11-b38e-224306cd7043', 'ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', 123, 'NUM 001', '2016-02-29', 'UNPAID', NULL, 'reynhart', '2016-02-28 20:35:21.592224+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('448e2691-7ef0-4c2b-8693-b61de9526ca0', 'ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', 123, 'NUM002', '2016-03-03', 'UNPAID', NULL, 'reynhart', '2016-03-02 17:53:15.470249+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('8efb5efb-d4ae-45ff-9a42-0d4197a2cefd', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 123, '123', '2016-03-06', 'UNPAID', NULL, 'reynhart', '2016-03-05 17:19:27.108685+07', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('79fcd4e8-c2c1-404e-80a5-3387ce42564c', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 128, 'NUM005', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 18:20:01.347963+07', NULL, NULL, true, 141);
INSERT INTO invoice VALUES ('d3de3bba-ec05-4c59-ba2c-535ada60b929', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 12300, 'NUM004', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 18:17:40.876929+07', 'reynhart', '2016-03-07 18:54:31.769384+07', true, 19030);
INSERT INTO invoice VALUES ('1d6b6f9a-ae36-4945-ad2d-0df3731332b1', 'efe7eb14-269e-4863-9fd3-a63cdc792f0b', 123123, 'NUM003', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 17:52:21.382018+07', 'reynhart', '2016-03-07 19:57:55.438942+07', false, 373123);
INSERT INTO invoice VALUES ('0981a1ff-caa9-4c47-a87d-66954483bfe6', '41092a86-6647-4820-9f39-d7b0f70969e3', 123123, 'NUM 001', '2016-03-15', 'UNPAID', NULL, 'reynhart', '2016-03-14 12:21:32.111664+07', NULL, NULL, true, 190435);
INSERT INTO invoice VALUES ('df0155e2-a24e-4c32-bf0c-f255f28cd2d3', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 123, 'NUM 001', '2016-03-15', 'UNPAID', NULL, 'reynhart', '2016-03-14 12:33:02.871173+07', NULL, NULL, true, 110135);
INSERT INTO invoice VALUES ('23371696-e99f-419a-bec6-4b92da6cfad3', 'efe7eb14-269e-4863-9fd3-a63cdc792f0b', 123123, 'API', '2016-03-17', 'UNPAID', NULL, 'reynhart', '2016-03-16 16:26:10.555336+07', NULL, NULL, true, 135435);
INSERT INTO invoice VALUES ('a5150eb5-6523-4c95-bbf4-71fff64fa707', '41092a86-6647-4820-9f39-d7b0f70969e3', 123123, 'NUM006', '2016-03-17', 'UNPAID', NULL, 'reynhart', '2016-03-16 16:36:26.664575+07', 'reynhart', '2016-03-16 16:36:42.379474+07', true, 135556);
INSERT INTO invoice VALUES ('2eaeaac3-7bb7-4320-99b3-76fa22a2d4c3', '303e8ff0-792e-4e6a-96eb-29cee8ab5cef', 100000, 'A001', '2016-03-01', 'UNPAID', NULL, 'reynhart', '2016-03-26 20:44:01.091456+07', 'reynhart', '2016-03-26 20:50:24.979976+07', false, 100000);
INSERT INTO invoice VALUES ('852a8968-5e57-4db6-990e-f13b9af1acf3', 'd0703ad5-1ad8-479e-9bad-3290e6f4fd44', 50000, 'A002', '2016-02-01', 'PAID', 'asdf', 'reynhart', '2016-03-26 20:51:01.107551+07', 'reynhart', '2016-03-26 21:02:48.762124+07', false, 50000);
INSERT INTO invoice VALUES ('a3a959fb-e8e3-41b5-8914-45ea6a88a800', '6285a2e9-6454-4b6f-8a15-b190342f01a0', 123, 'A11', '2016-04-05', 'UNPAID', NULL, 'reynhart', '2016-04-04 17:45:23.227441+07', 'reynhart', '2016-04-17 22:38:11.256786+07', true, 146);


--
-- TOC entry 2232 (class 0 OID 16439)
-- Dependencies: 189
-- Data for Name: invoice_item; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO invoice_item VALUES ('b24b7a0d-9023-4233-b2fa-d92e2cedeb42', 'c60e29ad-d5da-4e11-b38e-224306cd7043', 'ASDF', 11110);
INSERT INTO invoice_item VALUES ('754263d5-981e-4330-bd94-d427e19f6bd6', '448e2691-7ef0-4c2b-8693-b61de9526ca0', 'BENSIN', 20);
INSERT INTO invoice_item VALUES ('791312d6-4e83-4a99-9aa0-cdfec840f6aa', '448e2691-7ef0-4c2b-8693-b61de9526ca0', 'PARKIR', 10);
INSERT INTO invoice_item VALUES ('e1f6aaa8-d267-4cda-9c00-de8f412fcce2', 'd3de3bba-ec05-4c59-ba2c-535ada60b929', 'TOL1', 2000);
INSERT INTO invoice_item VALUES ('cef0e5de-01b8-44b3-8809-c6d856f0d239', 'd3de3bba-ec05-4c59-ba2c-535ada60b929', 'TAMBAH', 3000);
INSERT INTO invoice_item VALUES ('4746aa30-b073-4e94-97a4-3d2a880f2802', '1d6b6f9a-ae36-4945-ad2d-0df3731332b1', 'BENSIN', 200000);
INSERT INTO invoice_item VALUES ('774c1d0a-5ecd-4176-bacd-6c16fca07117', '1d6b6f9a-ae36-4945-ad2d-0df3731332b1', 'TOL', 50000);
INSERT INTO invoice_item VALUES ('b3b573d4-ff80-484f-a6ac-bd1ff9c82a07', '0981a1ff-caa9-4c47-a87d-66954483bfe6', 'Parkir', 50000);
INSERT INTO invoice_item VALUES ('af0ff058-0c22-404c-9fd5-62132b818428', 'df0155e2-a24e-4c32-bf0c-f255f28cd2d3', 'CATATAN', 0);
INSERT INTO invoice_item VALUES ('e07fb4a9-dda7-414d-8a2e-3d1a18e6b36c', 'df0155e2-a24e-4c32-bf0c-f255f28cd2d3', 'ASD', 100000);
INSERT INTO invoice_item VALUES ('7411ad54-9ad7-49cb-b435-2401dc1601ba', 'a5150eb5-6523-4c95-bbf4-71fff64fa707', 'X', 110);
INSERT INTO invoice_item VALUES ('cc680135-a0fd-4af0-a1d3-3bb58fc269cb', 'a3a959fb-e8e3-41b5-8914-45ea6a88a800', 'A', 10);


--
-- TOC entry 2238 (class 0 OID 33261)
-- Dependencies: 196
-- Data for Name: log_ws; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO log_ws VALUES ('2016-05-23 10:14:14.829469+07', '{"City":null,"StartRent":null,"FinishRent":null,"CarBrandName":null,"CarModelName":null,"CustomerName":null,"CustomerPhoneNumber":null,"PickupLocation":null,"Capacity":null}', '[]', '/api/Availability/CheckAvailability', NULL, NULL, '25925f8d-b2fe-4470-beb6-392c183ca833');
INSERT INTO log_ws VALUES ('2016-05-23 10:14:53.388665+07', '{"City":"bandung","StartRent":null,"FinishRent":null,"CarBrandName":null,"CarModelName":null,"CustomerName":null,"CustomerPhoneNumber":null,"PickupLocation":null,"Capacity":null}', '[]', '/api/Availability/CheckAvailability', NULL, NULL, 'e9d1177a-3367-4e93-a3b8-d21ea9ab5915');


--
-- TOC entry 2233 (class 0 OID 16442)
-- Dependencies: 190
-- Data for Name: owner; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'RK', 'Rekadia', 'superadmin', '2015-01-01 00:00:00+07', NULL, NULL, NULL, NULL, NULL, 1);
INSERT INTO owner VALUES ('f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'AM', 'Ameco', 'superadmin', '2015-02-02 00:00:00+07', NULL, NULL, NULL, NULL, NULL, 1);
INSERT INTO owner VALUES ('60c59a63-027f-484b-b8c4-4a03d2cd9b80', 'BDG', 'API Bandung', 'chandra', '2016-03-15 14:02:06.739474+07', 'chandra', '2016-03-15 14:02:06.739474+07', NULL, NULL, NULL, 1);
INSERT INTO owner VALUES ('9e9af49f-742a-40a2-9338-228e56060442', 'APB', 'API Bali', 'chandra', '2016-01-01 00:00:00+07', NULL, NULL, NULL, NULL, NULL, 1);
INSERT INTO owner VALUES ('157701c3-5f4f-4626-96bd-a905ababd469', 'BLI', 'API Bali Obsolete', 'chandra', '2016-03-15 14:00:45+07', 'chandra', '2016-03-15 14:02:16.915056+07', NULL, NULL, NULL, 1);
INSERT INTO owner VALUES ('7a683eb8-656d-4b4c-be71-157dd2328a64', 'APS', 'API Surabaya', 'chandra', '2016-01-01 00:00:00+07', NULL, NULL, NULL, NULL, NULL, 1);


--
-- TOC entry 2234 (class 0 OID 16448)
-- Dependencies: 191
-- Data for Name: owner_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'reynhart', 'e59fc4a1-11b7-44ee-9c75-73a645693269');
INSERT INTO owner_user VALUES ('60c59a63-027f-484b-b8c4-4a03d2cd9b80', 'api.bandung', '11650d11-4f26-4d80-a41c-81e2a424e613');
INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'yono', '5f475bd7-7491-4f64-a47b-92fe35c8f3ee');
INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'A10', 'f0953748-f205-4e2a-a424-9f32bebaea77');
INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'A11', 'da0d01ea-3cd9-4c03-94d9-669f15d98294');
INSERT INTO owner_user VALUES ('9e9af49f-742a-40a2-9338-228e56060442', 'api.bali', '729f08a7-ba32-4df1-b706-8c0dd57e35fc');
INSERT INTO owner_user VALUES ('7a683eb8-656d-4b4c-be71-157dd2328a64', 'api.surabaya', '76e5c791-a1a7-4096-8a37-fab64d5be4db');


--
-- TOC entry 2235 (class 0 OID 16451)
-- Dependencies: 192
-- Data for Name: rent; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO rent VALUES ('148fb2d5-c372-4bab-a313-423dfa838b32', '46d30608-492e-44a1-9cfe-37b5584ed4c1', 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'Kosambi', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-01 00:00:00+07', '2016-02-02 00:00:00+07', 300000, NULL, 'AM/ABCD', 'reynhart', '2015-12-31 00:00:00+07', NULL, NULL, NULL, NULL);
INSERT INTO rent VALUES ('5f4c62a0-58ea-444a-8623-00b72fc74563', 'e19b1f80-a675-42e0-a707-9da8c99730b5', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Penjemputan 2', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-01-01 16:00:00+07', '2016-01-02 16:00:00+07', 200000, 'catat<br />catat<br />catat', 'RK/156WW', 'reynhart', '2015-12-31 15:21:12.417955+07', 'reynhart', '2016-01-02 16:52:43.28185+07', 'GO', NULL);
INSERT INTO rent VALUES ('92b4b48d-37e1-4342-b1d7-96ab0ee5fc95', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-01 11:00:00+07', '2016-01-03 11:00:00+07', 250000, NULL, 'RK/PQ7A5', 'reynhart', '2016-01-02 10:33:02.262059+07', 'reynhart', '2016-01-02 16:53:07.226219+07', 'NEW', NULL);
INSERT INTO rent VALUES ('67d1b0f6-fa3d-4546-8f77-be893ae2291c', '199d978b-3eb1-4598-9613-84928b05f9fc', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84 1', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2016-01-02 15:00:00+07', '2016-01-03 15:00:00+07', 400000, NULL, 'RK/8D3QQ', 'reynhart', '2015-12-31 14:53:43.529644+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('a38ec800-ee61-4cc9-9e98-b4378a6e90ce', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Buah Batu', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-02 03:30:00+07', '2016-01-02 09:00:00+07', 150000, NULL, 'RK/4N00Q', 'reynhart', '2016-01-02 10:23:35.053617+07', 'reynhart', '2016-01-03 11:50:39.989791+07', 'CANCEL', 'tidak jadi');
INSERT INTO rent VALUES ('4683a7ed-44d8-4350-9729-298dea3ffaf9', 'e19b1f80-a675-42e0-a707-9da8c99730b5', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Medit 2', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-22 07:00:00+07', '2016-01-23 07:00:00+07', 123123, NULL, 'RK/2DVGZ', 'reynhart', '2016-01-21 06:19:25.826273+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('be0aa1e3-659a-40d2-806a-fff3701e62d6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Medit 1', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-21 06:00:00+07', '2016-01-22 03:00:00+07', 123123, NULL, 'RK/VKD04', 'reynhart', '2016-01-21 06:17:25.551393+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('68d405d4-cea6-41cb-b4a1-4e7ab4b9f0a6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-29 22:00:00+07', '2016-01-30 22:00:00+07', 123, NULL, 'RK/0FU50', 'reynhart', '2016-01-29 21:41:41.900242+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('cedd612d-0fcc-45b0-b749-1326f6345ba8', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', '123', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-01-29 22:00:00+07', '2016-01-30 22:00:00+07', 123, NULL, 'RK/P7X33', 'reynhart', '2016-01-29 21:43:08.187177+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('64fb50fd-3090-44ae-b981-4c00e9633d01', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'TSM', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-01-04 11:00:00+07', '2016-01-09 11:00:00+07', 350000, 'tes<br />tes', 'RK/9SZY0', 'reynhart', '2016-01-02 10:02:17.94857+07', 'reynhart', '2016-01-07 01:12:11.635528+07', 'CANCEL', 'tes<br />tes');
INSERT INTO rent VALUES ('ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-02-02 13:00:00+07', '2016-02-03 13:00:00+07', 123, NULL, 'RK/XIIFM', 'reynhart', '2016-02-02 12:36:47.720562+07', 'reynhart', '2016-02-17 21:56:40.233443+07', 'FINISH', NULL);
INSERT INTO rent VALUES ('41092a86-6647-4820-9f39-d7b0f70969e3', 'a0017e50-3e29-4095-8b83-cf0cfe142243', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Sawah Kurung', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-03-13 19:00:00+07', '2016-03-14 19:00:00+07', 123123, NULL, 'RK/4NCRT', 'reynhart', '2016-03-13 18:58:09.456571+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('693eec73-0db3-45dc-b0c8-50d003fd4370', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-03 19:00:00+07', '2016-01-04 19:00:00+07', 300000, NULL, 'RK/0O9P1', 'reynhart', '2016-01-03 18:28:55.775524+07', 'reynhart', '2016-01-03 18:31:30.366366+07', 'NEW', NULL);
INSERT INTO rent VALUES ('dc02193e-f1d8-492d-b34b-8abb2f6db496', 'de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'asdf', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2016-03-01 00:00:00+07', '2016-03-03 23:30:00+07', 123123, NULL, 'RK/7CQ78', 'reynhart', '2016-03-26 15:58:38.396114+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('8698ac1b-78c4-486a-9b7c-537077e057e6', 'de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'asdf', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '892b1090-d859-4a06-8922-932bdf7e9d40', '2016-03-09 16:00:00+07', '2016-03-10 23:30:00+07', 123123, NULL, 'RK/4D39C', 'reynhart', '2016-03-26 15:59:50.932952+07', NULL, NULL, 'GO', NULL);
INSERT INTO rent VALUES ('303e8ff0-792e-4e6a-96eb-29cee8ab5cef', 'e19b1f80-a675-42e0-a707-9da8c99730b5', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat 2', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '892b1090-d859-4a06-8922-932bdf7e9d40', '2016-03-11 17:00:00+07', '2016-03-11 17:00:00+07', 123, NULL, 'RK/XBYE0', 'reynhart', '2016-03-26 16:01:08.902937+07', 'reynhart', '2016-03-26 16:02:34.342467+07', 'NEW', NULL);
INSERT INTO rent VALUES ('eac003e1-98a3-404a-97cd-9004801b1c96', 'f7a3cd21-ce18-4683-a5fd-906c1859fd30', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'bandung', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', '7c264527-2ab4-4486-8ecf-402cd3ad8dba', '2016-06-01 07:00:00+07', '2016-06-01 17:00:00+07', 2000000, NULL, 'RK/7I1PV', 'reynhart', '2016-05-01 16:18:51.391064+07', NULL, NULL, 'GO', NULL);
INSERT INTO rent VALUES ('7edc2ccd-dc72-4f7e-a1b7-1ba69ddf7ab5', 'a0017e50-3e29-4095-8b83-cf0cfe142243', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'asdf', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', 'bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '2016-03-02 00:00:00+07', '2016-03-03 23:30:00+07', 123, NULL, 'RK/NJ94M', 'reynhart', '2016-03-26 17:06:26.976848+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('33c013a6-e25c-4a78-afea-2d1a2edc2c75', '854dca36-71eb-4dd1-a4ea-d627e6857eb5', '157701c3-5f4f-4626-96bd-a905ababd469', 'Jalan Veteran', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-16 00:00:00+07', '2016-04-17 00:00:00+07', 456, NULL, 'BLI/UGY6T', 'api.bali', '2016-04-15 23:01:31.21867+07', 'api.bali', '2016-04-15 23:07:44.896973+07', 'NEW', NULL);
INSERT INTO rent VALUES ('efe7eb14-269e-4863-9fd3-a63cdc792f0b', 'de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', 'bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '2016-03-11 00:00:00+07', '2016-03-11 23:00:00+07', 123123, NULL, 'RK/0U2SJ', 'reynhart', '2016-03-07 14:57:46.77433+07', 'reynhart', '2016-03-26 19:01:18.702818+07', 'NEW', NULL);
INSERT INTO rent VALUES ('366c10ba-063f-4409-a3ac-9d5da31c01b1', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'ads', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', 'bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '2016-03-04 00:00:00+07', '2016-03-05 23:30:00+07', 123, NULL, 'RK/4V9ER', 'reynhart', '2016-03-26 17:08:11.264825+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('d0703ad5-1ad8-479e-9bad-3290e6f4fd44', 'a0017e50-3e29-4095-8b83-cf0cfe142243', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'asdf', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '892b1090-d859-4a06-8922-932bdf7e9d40', '2016-02-29 17:00:00+07', '2016-03-03 17:00:00+07', 123, NULL, 'RK/UEDUG', 'reynhart', '2016-03-26 16:00:35.40385+07', NULL, NULL, 'FINISH', NULL);
INSERT INTO rent VALUES ('60e8a355-702a-4c76-b8c0-942f6e413896', '199d978b-3eb1-4598-9613-84928b05f9fc', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', 'ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2015-12-02 15:00:00+07', '2015-12-17 02:00:00+07', 300000, 'catatan<br />123', 'RK/FH145', 'reynhart', '2015-12-31 14:49:41.641809+07', 'reynhart', '2016-03-27 14:59:32.549428+07', 'NEW', NULL);
INSERT INTO rent VALUES ('4fc69aeb-acc4-4aa6-a2e8-734eddf3501d', '854dca36-71eb-4dd1-a4ea-d627e6857eb5', '157701c3-5f4f-4626-96bd-a905ababd469', 'ASDF', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-04-02 23:00:00+07', '2016-04-06 05:00:00+07', 123, NULL, 'BLI/GC90M', 'api.bali', '2016-04-15 22:38:09.016957+07', 'api.bali', '2016-04-15 23:11:47.58349+07', 'NEW', NULL);
INSERT INTO rent VALUES ('6285a2e9-6454-4b6f-8a15-b190342f01a0', 'a0017e50-3e29-4095-8b83-cf0cfe142243', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'adf', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '4b96c548-f6b3-420a-8a20-000388bc0bb4', '2015-12-17 16:00:00+07', '2015-12-18 17:00:00+07', 123, NULL, 'RK/KFY9U', 'reynhart', '2016-03-27 15:00:33.776816+07', 'reynhart', '2016-03-27 15:07:20.855954+07', 'CANCEL', 'asdf');
INSERT INTO rent VALUES ('24696b78-ae03-4ba6-903f-e4cc5cbff8d0', 'e5e5491b-e655-4dd9-a78e-c990e820670a', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'kll', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '913e8705-f367-4e00-a6a5-6b403156c79b', '7c264527-2ab4-4486-8ecf-402cd3ad8dba', '2015-12-30 17:00:00+07', '2016-02-02 17:00:00+07', 2, NULL, 'RK/W6PA5', 'reynhart', '2016-05-01 16:26:40.87052+07', 'reynhart', '2016-05-22 15:21:07.592362+07', 'GO', 'xdfg');
INSERT INTO rent VALUES ('77573b70-6112-4b0a-bb01-c15af6e8fdfb', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '913e8705-f367-4e00-a6a5-6b403156c79b', '7c264527-2ab4-4486-8ecf-402cd3ad8dba', '2016-06-01 15:00:00+07', '2016-06-04 17:00:00+07', 12000, NULL, 'RK/93YZ0', 'reynhart', '2016-05-01 16:23:59.258927+07', 'reynhart', '2016-05-03 11:13:05.613723+07', 'CANCEL', 'tes');
INSERT INTO rent VALUES ('1911ff33-f1ec-4631-81ef-ac436efd582a', 'de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'malamg', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '913e8705-f367-4e00-a6a5-6b403156c79b', '7c264527-2ab4-4486-8ecf-402cd3ad8dba', '2016-01-09 17:00:00+07', '2016-02-02 21:00:00+07', 5, NULL, 'RK/CDIPO', 'reynhart', '2016-05-01 16:25:47.403235+07', 'reynhart', '2016-05-01 18:49:08.349656+07', 'FINISH', NULL);
INSERT INTO rent VALUES ('33b48b21-6f74-4a7b-be50-3af52b02e2e7', '0a54ef48-eccf-46c8-b503-7dfc1de2f70d', '157701c3-5f4f-4626-96bd-a905ababd469', 'sawah', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-16 00:00:00+07', '2016-04-17 00:00:00+07', 122, NULL, 'BLI/HSSVB', 'api.bali', '2016-04-15 23:10:10.846379+07', 'api.bali', '2016-04-15 12:03:51.768683+07', 'CANCEL', 'ccc');
INSERT INTO rent VALUES ('e64c58e9-9c63-4e92-9e64-da38769b865a', '33717cf2-ea31-4849-8aa1-a53c6806de61', '157701c3-5f4f-4626-96bd-a905ababd469', 'ADSF', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-16 00:00:00+07', '2016-04-17 00:00:00+07', 123, NULL, 'BLI/RRLTO', 'api.bali', '2016-04-15 23:09:07.045908+07', 'api.bali', '2016-04-15 12:04:10.385003+07', 'CANCEL', 'bbb');
INSERT INTO rent VALUES ('90e5161c-f6e3-42c3-ac86-67fbc481f6ca', 'a8ea9dc1-4db7-4cc8-a0e3-6dada4ad3f5f', '157701c3-5f4f-4626-96bd-a905ababd469', 'Jalan Veteran 123', NULL, '3946280e-8a82-41ee-80cd-748841e175b6', NULL, '2016-04-15 13:00:00+07', '2016-04-16 13:00:00+07', 123, NULL, 'BLI/S0TDM', 'api.bali', '2016-04-15 12:47:52.213437+07', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('141f52e1-d3b5-41d9-a69b-01d0ee56709a', 'f7a3cd21-ce18-4683-a5fd-906c1859fd30', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'dadas', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', '892b1090-d859-4a06-8922-932bdf7e9d40', '2016-05-01 07:00:00+07', '2016-06-01 17:00:00+07', 120000, NULL, 'RK/86BEL', 'reynhart', '2016-05-01 16:21:05.112891+07', 'reynhart', '2016-05-23 08:46:02.084933+07', 'GO', NULL);
INSERT INTO rent VALUES ('35fceb79-845e-4244-9225-eda742c81acc', '3186759c-b0d8-432b-b76d-ef4b1dfc91ef', '157701c3-5f4f-4626-96bd-a905ababd469', 'medan merdeka', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-01 18:00:00+07', '2016-04-01 19:00:00+07', 0, NULL, 'BLI/6ECOH', 'aerotrans', '2016-05-12 08:50:18.899382+07', 'aerotrans', '2016-05-12 08:50:18.899883+07', 'NEW', NULL);
INSERT INTO rent VALUES ('2a555df5-abea-4e9d-bb9d-401f5285eac7', '3186759c-b0d8-432b-b76d-ef4b1dfc91ef', '157701c3-5f4f-4626-96bd-a905ababd469', 'medan merdeka', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-02 20:00:00+07', '2016-04-02 21:00:00+07', 0, NULL, 'BLI/PG54C', 'aerotrans', '2016-05-12 08:53:07.639224+07', 'aerotrans', '2016-05-12 08:53:07.639224+07', 'NEW', NULL);
INSERT INTO rent VALUES ('06c81690-1622-4b3e-a53e-689e4c6b5cd6', '4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Alamat', '807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', '913e8705-f367-4e00-a6a5-6b403156c79b', '7c264527-2ab4-4486-8ecf-402cd3ad8dba', '2016-01-03 19:00:00+07', '2016-01-04 19:00:00+07', 300000, NULL, 'RK/YN32U', 'reynhart', '2016-01-03 18:25:17.407034+07', 'reynhart', '2016-05-01 18:55:34.939609+07', 'NEW', NULL);
INSERT INTO rent VALUES ('f6223b60-f72d-446e-90bb-611b134a9b45', '32892c3c-a935-4599-b251-2102caf4ff35', '9e9af49f-742a-40a2-9338-228e56060442', 'medan merdeka', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', NULL, '2016-04-01 20:00:00+07', '2016-04-01 21:00:00+07', 0, NULL, 'APB/M0T7T', 'aerotrans', '2016-05-13 10:00:15.904627+07', 'aerotrans', '2016-05-13 10:00:15.906632+07', 'NEW', NULL);


--
-- TOC entry 2236 (class 0 OID 16457)
-- Dependencies: 193
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
INSERT INTO rent_code VALUES ('RK/0U2SJ');
INSERT INTO rent_code VALUES ('RK/4NCRT');
INSERT INTO rent_code VALUES ('RK/7CQ78');
INSERT INTO rent_code VALUES ('RK/4D39C');
INSERT INTO rent_code VALUES ('RK/UEDUG');
INSERT INTO rent_code VALUES ('RK/XBYE0');
INSERT INTO rent_code VALUES ('RK/NJ94M');
INSERT INTO rent_code VALUES ('RK/4V9ER');
INSERT INTO rent_code VALUES ('RK/KFY9U');
INSERT INTO rent_code VALUES ('BLI/GC90M');
INSERT INTO rent_code VALUES ('BLI/UGY6T');
INSERT INTO rent_code VALUES ('BLI/RRLTO');
INSERT INTO rent_code VALUES ('BLI/HSSVB');
INSERT INTO rent_code VALUES ('BLI/S0TDM');
INSERT INTO rent_code VALUES ('RK/7I1PV');
INSERT INTO rent_code VALUES ('RK/86BEL');
INSERT INTO rent_code VALUES ('RK/93YZ0');
INSERT INTO rent_code VALUES ('RK/CDIPO');
INSERT INTO rent_code VALUES ('RK/W6PA5');
INSERT INTO rent_code VALUES ('BLI/HJFR8');
INSERT INTO rent_code VALUES ('BLI/6ECOH');
INSERT INTO rent_code VALUES ('BLI/PG54C');
INSERT INTO rent_code VALUES ('APB/M0T7T');


--
-- TOC entry 2237 (class 0 OID 33251)
-- Dependencies: 195
-- Data for Name: rent_position; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO rent_position VALUES ('573b034f-53f6-423b-a2d5-2e613433dec7', '24696b78-ae03-4ba6-903f-e4cc5cbff8d0', 'reynhart', '2016-05-22 15:22:15.355809+07', -6.893694, 107.602619);
INSERT INTO rent_position VALUES ('a5cf11b3-5fe3-4fdc-998a-787fddc215c1', '141f52e1-d3b5-41d9-a69b-01d0ee56709a', 'reynhart', '2016-05-23 00:00:00+07', -6.8914770000000001, 107.596976);


--
-- TOC entry 2079 (class 2606 OID 16466)
-- Name: Primary Key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner
    ADD CONSTRAINT "Primary Key" PRIMARY KEY (id);


--
-- TOC entry 2061 (class 2606 OID 16468)
-- Name: pk_car; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT pk_car PRIMARY KEY (id);


--
-- TOC entry 2063 (class 2606 OID 16470)
-- Name: pk_car_brand; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_brand
    ADD CONSTRAINT pk_car_brand PRIMARY KEY (id);


--
-- TOC entry 2065 (class 2606 OID 16472)
-- Name: pk_car_type; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT pk_car_type PRIMARY KEY (id);


--
-- TOC entry 2091 (class 2606 OID 33285)
-- Name: pk_city; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY city
    ADD CONSTRAINT pk_city PRIMARY KEY (id);


--
-- TOC entry 2071 (class 2606 OID 16474)
-- Name: pk_cost; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT pk_cost PRIMARY KEY (id);


--
-- TOC entry 2073 (class 2606 OID 16476)
-- Name: pk_cost_item; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT pk_cost_item PRIMARY KEY (id);


--
-- TOC entry 2067 (class 2606 OID 16478)
-- Name: pk_customer; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT pk_customer PRIMARY KEY (id);


--
-- TOC entry 2069 (class 2606 OID 16480)
-- Name: pk_driver; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT pk_driver PRIMARY KEY (id);


--
-- TOC entry 2075 (class 2606 OID 16482)
-- Name: pk_invoice; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT pk_invoice PRIMARY KEY (id);


--
-- TOC entry 2077 (class 2606 OID 16484)
-- Name: pk_invoice_item; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice_item
    ADD CONSTRAINT pk_invoice_item PRIMARY KEY (id);


--
-- TOC entry 2089 (class 2606 OID 33268)
-- Name: pk_log_ws; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY log_ws
    ADD CONSTRAINT pk_log_ws PRIMARY KEY (id);


--
-- TOC entry 2081 (class 2606 OID 16486)
-- Name: pk_owner_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT pk_owner_user PRIMARY KEY (id);


--
-- TOC entry 2083 (class 2606 OID 16488)
-- Name: pk_rent; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT pk_rent PRIMARY KEY (id);


--
-- TOC entry 2085 (class 2606 OID 16490)
-- Name: pk_rent_code; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_code
    ADD CONSTRAINT pk_rent_code PRIMARY KEY (code);


--
-- TOC entry 2087 (class 2606 OID 33255)
-- Name: pk_rent_position; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_position
    ADD CONSTRAINT pk_rent_position PRIMARY KEY (id);


--
-- TOC entry 2092 (class 2606 OID 16491)
-- Name: fk_car_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2094 (class 2606 OID 16496)
-- Name: fk_car_model_car_brand; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT fk_car_model_car_brand FOREIGN KEY (id_car_brand) REFERENCES car_brand(id);


--
-- TOC entry 2093 (class 2606 OID 16501)
-- Name: fk_car_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2098 (class 2606 OID 16506)
-- Name: fk_cost_item_cost; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT fk_cost_item_cost FOREIGN KEY (id_expense) REFERENCES expense(id);


--
-- TOC entry 2097 (class 2606 OID 16511)
-- Name: fk_cost_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT fk_cost_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2095 (class 2606 OID 16516)
-- Name: fk_customer_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT fk_customer_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2096 (class 2606 OID 16521)
-- Name: fk_driver_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT fk_driver_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2100 (class 2606 OID 16526)
-- Name: fk_invoice_invoice_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice_item
    ADD CONSTRAINT fk_invoice_invoice_item FOREIGN KEY (id_invoice) REFERENCES invoice(id);


--
-- TOC entry 2099 (class 2606 OID 16531)
-- Name: fk_invoice_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT fk_invoice_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2102 (class 2606 OID 16536)
-- Name: fk_ou_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT fk_ou_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2101 (class 2606 OID 33289)
-- Name: fk_owner_city; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner
    ADD CONSTRAINT fk_owner_city FOREIGN KEY (id_city) REFERENCES city(id);


--
-- TOC entry 2103 (class 2606 OID 16541)
-- Name: fk_rent_car; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car FOREIGN KEY (id_car) REFERENCES car(id);


--
-- TOC entry 2104 (class 2606 OID 16546)
-- Name: fk_rent_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2105 (class 2606 OID 16551)
-- Name: fk_rent_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_customer FOREIGN KEY (id_customer) REFERENCES customer(id);


--
-- TOC entry 2106 (class 2606 OID 16556)
-- Name: fk_rent_driver; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_driver FOREIGN KEY (id_driver) REFERENCES driver(id);


--
-- TOC entry 2107 (class 2606 OID 16561)
-- Name: fk_rent_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2108 (class 2606 OID 33256)
-- Name: fk_rent_position_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_position
    ADD CONSTRAINT fk_rent_position_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2247 (class 0 OID 0)
-- Dependencies: 7
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2016-05-24 10:27:19

--
-- PostgreSQL database dump complete
--

