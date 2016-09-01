--
-- PostgreSQL database dump
--

-- Dumped from database version 9.5rc1
-- Dumped by pg_dump version 9.5rc1

-- Started on 2016-03-16 10:30:59

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 194 (class 3079 OID 12355)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2221 (class 0 OID 0)
-- Dependencies: 194
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- TOC entry 196 (class 1255 OID 25061)
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
-- TOC entry 195 (class 1255 OID 25060)
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
    id_car_brand uuid NOT NULL,
    capacity smallint DEFAULT 0 NOT NULL
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
    id_owner uuid NOT NULL,
    title character varying(4),
    company character varying(32),
    photo character varying(128)
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
    id_owner uuid NOT NULL,
    work_start_date date,
    photo character varying(128)
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
    updated_time timestamp with time zone,
    ppn boolean DEFAULT false NOT NULL,
    total integer DEFAULT 0 NOT NULL
);


ALTER TABLE invoice OWNER TO postgres;

--
-- TOC entry 193 (class 1259 OID 25121)
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
-- TOC entry 2205 (class 0 OID 16451)
-- Dependencies: 184
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


--
-- TOC entry 2206 (class 0 OID 16454)
-- Dependencies: 185
-- Data for Name: car_brand; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_brand VALUES ('eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 'Toyota');
INSERT INTO car_brand VALUES ('9572d6ec-a2df-4407-b2f8-f06d61dd122f', 'Honda');


--
-- TOC entry 2204 (class 0 OID 16441)
-- Dependencies: 183
-- Data for Name: car_model; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO car_model VALUES ('545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', 'Avanza', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 0);
INSERT INTO car_model VALUES ('125700cd-f447-4a6c-9e19-206c36a8369e', 'Innova', 'eac29cfa-fc1b-4e32-85dd-804c5eb3b576', 0);
INSERT INTO car_model VALUES ('913e8705-f367-4e00-a6a5-6b403156c79b', 'Freed', '9572d6ec-a2df-4407-b2f8-f06d61dd122f', 0);
INSERT INTO car_model VALUES ('3946280e-8a82-41ee-80cd-748841e175b6', 'Jazz', '9572d6ec-a2df-4407-b2f8-f06d61dd122f', 0);


--
-- TOC entry 2202 (class 0 OID 16415)
-- Dependencies: 181
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO customer VALUES ('46d30608-492e-44a1-9cfe-37b5584ed4c1', 'Hutami', 'Corporate', '08112210960', NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', NULL, NULL, NULL);
INSERT INTO customer VALUES ('d3e5b635-7c36-47bd-b215-41896ccc523f', 'Bolu', NULL, NULL, 'Sawah Kurung', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('199d978b-3eb1-4598-9613-84928b05f9fc', 'Lauw Siu Hung', 'CORPORATE', '08122356566', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('e9b9d948-0a77-4089-aca0-324b913d0d34', 'Chandra', 'CORPORATE', '085722925900', 'Veteran 84', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('e19b1f80-a675-42e0-a707-9da8c99730b5', 'Ronny', NULL, '081111113', 'Alamat 2', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('e5e5491b-e655-4dd9-a78e-c990e820670a', 'Michael James', NULL, '081111114', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('4fb2ad96-9acf-490e-a907-9822a7c0be6b', 'Andreas', 'PERSONAL', '081111112;082222222', 'Alamat', NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('a0017e50-3e29-4095-8b83-cf0cfe142243', 'Kurnia', NULL, '081111116', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);
INSERT INTO customer VALUES ('de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'Dwijayanti', 'PERSONAL', '081111115', NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL, NULL);


--
-- TOC entry 2203 (class 0 OID 16428)
-- Dependencies: 182
-- Data for Name: driver; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO driver VALUES ('ff0c7f36-6c3b-413b-9311-4d4a0149cfc4', 'Suryono', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL);
INSERT INTO driver VALUES ('807b9a6c-7c5b-44f8-ae33-a07b424dc8a2', 'Johnny', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'd021a28b-6aca-4bbf-930c-935ff63691e9', NULL, NULL);
INSERT INTO driver VALUES ('1d4a3bfe-a585-42e8-9b67-17f4309108e6', 'Bobon', 'PARTTIME', NULL, NULL, NULL, NULL, NULL, 'f9a31c82-f545-45b4-b25e-90d5c023d3a5', NULL, NULL);


--
-- TOC entry 2211 (class 0 OID 25082)
-- Dependencies: 190
-- Data for Name: expense; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO expense VALUES ('b3cb5043-26ea-4966-9385-6b8cb9ee42e3', '06c81690-1622-4b3e-a53e-689e4c6b5cd6', '2016-02-27', 'reynhart', '2016-02-27 00:00:00+00', NULL, NULL);
INSERT INTO expense VALUES ('28f3219c-2fac-4a30-bb5f-f3c7f8612e41', '67d1b0f6-fa3d-4546-8f77-be893ae2291c', '2016-02-27', 'reynhart', '2016-02-27 00:00:00+00', NULL, NULL);
INSERT INTO expense VALUES ('499c79dc-70d2-4f67-a024-29f4d60064d1', '60e8a355-702a-4c76-b8c0-942f6e413896', '2016-02-27', 'reynhart', '2016-02-26 00:00:00+00', NULL, NULL);
INSERT INTO expense VALUES ('12e0dddc-2a98-4500-a514-78dec3856c5a', 'efe7eb14-269e-4863-9fd3-a63cdc792f0b', '2016-03-14', 'reynhart', '2016-03-14 05:20:31.175178+00', 'reynhart', '2016-03-15 10:10:34.676988+00');


--
-- TOC entry 2212 (class 0 OID 25100)
-- Dependencies: 191
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
-- TOC entry 2208 (class 0 OID 16521)
-- Dependencies: 187
-- Data for Name: invoice; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO invoice VALUES ('885e527e-8b3d-4c70-aa23-9c36ba9c32c3', 'a38ec800-ee61-4cc9-9e98-b4378a6e90ce', 100000, '123', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 04:41:29.020277+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('7d3e7653-e7c6-472c-8bc8-7fa62a3453b9', '92b4b48d-37e1-4342-b1d7-96ab0ee5fc95', 200000, '111', '2016-01-04', 'PAID', NULL, 'reynhart', '2016-01-03 04:41:54.43073+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('ce4e6780-21d3-4865-a39e-c108e82c0996', '64fb50fd-3090-44ae-b981-4c00e9633d01', 150000, '112', '2016-01-04', 'UNPAID', NULL, 'reynhart', '2016-01-03 04:42:13.214805+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('3eaaadac-6677-404f-a818-accdd2a8ed22', '68d405d4-cea6-41cb-b4a1-4e7ab4b9f0a6', 1233, 'INV-0131-B', '2016-02-02', 'PAID', NULL, 'reynhart', '2016-01-31 05:20:32.63084+00', 'reynhart', '2016-01-31 05:21:50.977322+00', false, 0);
INSERT INTO invoice VALUES ('c60e29ad-d5da-4e11-b38e-224306cd7043', 'ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', 123, 'NUM 001', '2016-02-29', 'UNPAID', NULL, 'reynhart', '2016-02-28 13:35:21.592224+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('448e2691-7ef0-4c2b-8693-b61de9526ca0', 'ec7effd9-81ce-4a6a-aa18-cfe68f6e4c99', 123, 'NUM002', '2016-03-03', 'UNPAID', NULL, 'reynhart', '2016-03-02 10:53:15.470249+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('8efb5efb-d4ae-45ff-9a42-0d4197a2cefd', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 123, '123', '2016-03-06', 'UNPAID', NULL, 'reynhart', '2016-03-05 10:19:27.108685+00', NULL, NULL, false, 0);
INSERT INTO invoice VALUES ('79fcd4e8-c2c1-404e-80a5-3387ce42564c', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 128, 'NUM005', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 11:20:01.347963+00', NULL, NULL, true, 141);
INSERT INTO invoice VALUES ('d3de3bba-ec05-4c59-ba2c-535ada60b929', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 12300, 'NUM004', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 11:17:40.876929+00', 'reynhart', '2016-03-07 11:54:31.769384+00', true, 19030);
INSERT INTO invoice VALUES ('1d6b6f9a-ae36-4945-ad2d-0df3731332b1', 'efe7eb14-269e-4863-9fd3-a63cdc792f0b', 123123, 'NUM003', '2016-03-08', 'UNPAID', NULL, 'reynhart', '2016-03-07 10:52:21.382018+00', 'reynhart', '2016-03-07 12:57:55.438942+00', false, 373123);
INSERT INTO invoice VALUES ('0981a1ff-caa9-4c47-a87d-66954483bfe6', '41092a86-6647-4820-9f39-d7b0f70969e3', 123123, 'NUM 001', '2016-03-15', 'UNPAID', NULL, 'reynhart', '2016-03-14 05:21:32.111664+00', NULL, NULL, true, 190435);
INSERT INTO invoice VALUES ('df0155e2-a24e-4c32-bf0c-f255f28cd2d3', 'cedd612d-0fcc-45b0-b749-1326f6345ba8', 123, 'NUM 001', '2016-03-15', 'UNPAID', NULL, 'reynhart', '2016-03-14 05:33:02.871173+00', NULL, NULL, true, 110135);


--
-- TOC entry 2213 (class 0 OID 25121)
-- Dependencies: 193
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


--
-- TOC entry 2201 (class 0 OID 16394)
-- Dependencies: 180
-- Data for Name: owner; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'RK', 'Rekadia', 'superadmin', '2014-12-31 17:00:00+00', NULL, NULL, NULL, NULL, NULL);
INSERT INTO owner VALUES ('f9a31c82-f545-45b4-b25e-90d5c023d3a5', 'AM', 'Ameco', 'superadmin', '2015-02-01 17:00:00+00', NULL, NULL, NULL, NULL, NULL);
INSERT INTO owner VALUES ('60c59a63-027f-484b-b8c4-4a03d2cd9b80', 'BDG', 'API Bandung', 'chandra', '2016-03-15 07:02:06.739474+00', 'chandra', '2016-03-15 07:02:06.739474+00', NULL, NULL, NULL);
INSERT INTO owner VALUES ('157701c3-5f4f-4626-96bd-a905ababd469', 'BLI', 'API Bali', 'chandra', '2016-03-15 07:00:45+00', 'chandra', '2016-03-15 07:02:16.915056+00', NULL, NULL, NULL);


--
-- TOC entry 2209 (class 0 OID 16801)
-- Dependencies: 188
-- Data for Name: owner_user; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO owner_user VALUES ('d021a28b-6aca-4bbf-930c-935ff63691e9', 'reynhart', 'e59fc4a1-11b7-44ee-9c75-73a645693269');
INSERT INTO owner_user VALUES ('157701c3-5f4f-4626-96bd-a905ababd469', 'api.bali', '0da29dab-4e75-415d-826b-56cc0ea91816');
INSERT INTO owner_user VALUES ('60c59a63-027f-484b-b8c4-4a03d2cd9b80', 'api.bandung', '11650d11-4f26-4d80-a41c-81e2a424e613');


--
-- TOC entry 2207 (class 0 OID 16471)
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
INSERT INTO rent VALUES ('41092a86-6647-4820-9f39-d7b0f70969e3', 'a0017e50-3e29-4095-8b83-cf0cfe142243', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Sawah Kurung', NULL, '913e8705-f367-4e00-a6a5-6b403156c79b', NULL, '2016-03-13 12:00:00+00', '2016-03-14 12:00:00+00', 123123, NULL, 'RK/4NCRT', 'reynhart', '2016-03-13 11:58:09.456571+00', NULL, NULL, 'NEW', NULL);
INSERT INTO rent VALUES ('693eec73-0db3-45dc-b0c8-50d003fd4370', 'e9b9d948-0a77-4089-aca0-324b913d0d34', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', NULL, '545f7a1c-cbd0-44c5-b25a-1f4df8749b5c', NULL, '2016-01-03 12:00:00+00', '2016-01-04 12:00:00+00', 300000, NULL, 'RK/0O9P1', 'reynhart', '2016-01-03 11:28:55.775524+00', 'reynhart', '2016-01-03 11:31:30.366366+00', 'NEW', NULL);
INSERT INTO rent VALUES ('efe7eb14-269e-4863-9fd3-a63cdc792f0b', 'de15ebf4-c7e7-4166-ab4f-a6eda40d4b06', 'd021a28b-6aca-4bbf-930c-935ff63691e9', 'Veteran 84', NULL, '125700cd-f447-4a6c-9e19-206c36a8369e', 'bdbb36a4-ff64-4e68-9d07-4ac52e48d821', '2016-03-07 08:00:00+00', '2016-03-08 08:00:00+00', 123123, NULL, 'RK/0U2SJ', 'reynhart', '2016-03-07 07:57:46.77433+00', 'reynhart', '2016-03-15 10:17:36.544117+00', 'NEW', NULL);


--
-- TOC entry 2210 (class 0 OID 25062)
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
INSERT INTO rent_code VALUES ('RK/0U2SJ');
INSERT INTO rent_code VALUES ('RK/4NCRT');


--
-- TOC entry 2046 (class 2606 OID 16402)
-- Name: Primary Key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner
    ADD CONSTRAINT "Primary Key" PRIMARY KEY (id);


--
-- TOC entry 2054 (class 2606 OID 16465)
-- Name: pk_car; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT pk_car PRIMARY KEY (id);


--
-- TOC entry 2056 (class 2606 OID 16458)
-- Name: pk_car_brand; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_brand
    ADD CONSTRAINT pk_car_brand PRIMARY KEY (id);


--
-- TOC entry 2052 (class 2606 OID 16445)
-- Name: pk_car_type; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT pk_car_type PRIMARY KEY (id);


--
-- TOC entry 2066 (class 2606 OID 25086)
-- Name: pk_cost; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT pk_cost PRIMARY KEY (id);


--
-- TOC entry 2068 (class 2606 OID 25108)
-- Name: pk_cost_item; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT pk_cost_item PRIMARY KEY (id);


--
-- TOC entry 2048 (class 2606 OID 16422)
-- Name: pk_customer; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT pk_customer PRIMARY KEY (id);


--
-- TOC entry 2050 (class 2606 OID 16435)
-- Name: pk_driver; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT pk_driver PRIMARY KEY (id);


--
-- TOC entry 2060 (class 2606 OID 16529)
-- Name: pk_invoice; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT pk_invoice PRIMARY KEY (id);


--
-- TOC entry 2070 (class 2606 OID 25125)
-- Name: pk_invoice_item; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice_item
    ADD CONSTRAINT pk_invoice_item PRIMARY KEY (id);


--
-- TOC entry 2062 (class 2606 OID 16808)
-- Name: pk_owner_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT pk_owner_user PRIMARY KEY (id);


--
-- TOC entry 2058 (class 2606 OID 16475)
-- Name: pk_rent; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT pk_rent PRIMARY KEY (id);


--
-- TOC entry 2064 (class 2606 OID 25066)
-- Name: pk_rent_code; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent_code
    ADD CONSTRAINT pk_rent_code PRIMARY KEY (code);


--
-- TOC entry 2074 (class 2606 OID 16466)
-- Name: fk_car_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2073 (class 2606 OID 16459)
-- Name: fk_car_model_car_brand; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car_model
    ADD CONSTRAINT fk_car_model_car_brand FOREIGN KEY (id_car_brand) REFERENCES car_brand(id);


--
-- TOC entry 2075 (class 2606 OID 16868)
-- Name: fk_car_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY car
    ADD CONSTRAINT fk_car_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2084 (class 2606 OID 25109)
-- Name: fk_cost_item_cost; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense_item
    ADD CONSTRAINT fk_cost_item_cost FOREIGN KEY (id_expense) REFERENCES expense(id);


--
-- TOC entry 2083 (class 2606 OID 25087)
-- Name: fk_cost_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY expense
    ADD CONSTRAINT fk_cost_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2071 (class 2606 OID 16423)
-- Name: fk_customer_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY customer
    ADD CONSTRAINT fk_customer_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2072 (class 2606 OID 16436)
-- Name: fk_driver_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY driver
    ADD CONSTRAINT fk_driver_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2085 (class 2606 OID 25126)
-- Name: fk_invoice_invoice_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice_item
    ADD CONSTRAINT fk_invoice_invoice_item FOREIGN KEY (id_invoice) REFERENCES invoice(id);


--
-- TOC entry 2081 (class 2606 OID 16530)
-- Name: fk_invoice_rent; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY invoice
    ADD CONSTRAINT fk_invoice_rent FOREIGN KEY (id_rent) REFERENCES rent(id);


--
-- TOC entry 2082 (class 2606 OID 16809)
-- Name: fk_ou_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY owner_user
    ADD CONSTRAINT fk_ou_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2080 (class 2606 OID 16499)
-- Name: fk_rent_car; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car FOREIGN KEY (id_car) REFERENCES car(id);


--
-- TOC entry 2079 (class 2606 OID 16494)
-- Name: fk_rent_car_model; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_car_model FOREIGN KEY (id_car_model) REFERENCES car_model(id);


--
-- TOC entry 2076 (class 2606 OID 16476)
-- Name: fk_rent_customer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_customer FOREIGN KEY (id_customer) REFERENCES customer(id);


--
-- TOC entry 2078 (class 2606 OID 16489)
-- Name: fk_rent_driver; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_driver FOREIGN KEY (id_driver) REFERENCES driver(id);


--
-- TOC entry 2077 (class 2606 OID 16484)
-- Name: fk_rent_owner; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY rent
    ADD CONSTRAINT fk_rent_owner FOREIGN KEY (id_owner) REFERENCES owner(id);


--
-- TOC entry 2220 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


-- Completed on 2016-03-16 10:31:02

--
-- PostgreSQL database dump complete
--

