-- Create the table if it does not exist
CREATE TABLE IF NOT EXISTS public."Users"
(
    "UserID" uuid NOT NULL,
    "PersonName" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Email" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Password" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Gender" character varying(15) COLLATE pg_catalog."default",
    CONSTRAINT "Users_pkey" PRIMARY KEY ("UserID")
);

-- Insert 3 sample users with random GUIDs
INSERT INTO public."Users" ("UserID", "PersonName", "Email", "Password", "Gender") VALUES 
(
    'f47ac10b-58cc-4372-a567-0e02b2c3d479'::uuid,  
    'Hesham',
    'hesham@gmail.com',
    'admin',
    'Male'
),
(
    'b7c5a6d8-9e2f-4a3b-8c1d-5f6e7a8b9c0d'::uuid, 
    'admin',
    'admin@gmail.com',
    'password123',
    'Male'
),
(
    'a1b2c3d4-e5f6-7890-abcd-ef1234567890'::uuid,  
    'Sara',
    'sara@gmail.com',
    'test123',
    'Female'
);

SELECT "UserID", "PersonName", "Email", "Gender" FROM public."Users";