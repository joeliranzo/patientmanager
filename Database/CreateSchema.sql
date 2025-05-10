CREATE TABLE patients (
    id INT PRIMARY KEY IDENTITY(1,1),
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    social_security_number_encrypted VARBINARY(MAX) NOT NULL,
    address NVARCHAR(255),
    phone_number NVARCHAR(20),
    email NVARCHAR(100) UNIQUE,
    created_date DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    modified_date DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);

CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(100) NOT NULL UNIQUE,
    password_hash NVARCHAR(MAX) NOT NULL,
    role NVARCHAR(50) NOT NULL DEFAULT 'User',
    created_date DATETIME2 DEFAULT GETUTCDATE() NOT NULL
	modified_date DATETIME2 DEFAULT GETUTCDATE() NOT NULL
);