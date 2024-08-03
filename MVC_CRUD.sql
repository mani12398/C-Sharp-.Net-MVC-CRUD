USE [MVC_CRUD];

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY (1,1),
    Username VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) UNIQUE NOT NULL,
	Mobile_No varchar(255) UNIQUE NOT NULL,
    UserPassword varchar(255) NOT NULL,
	RegistrationDate DATE NOT NULL,
	RegistrationTime TIME NOT NULL,
	UserRole VARCHAR (255) NOT NULL,
		
);

CREATE TABLE UserLogins (

    Id INT PRIMARY KEY IDENTITY (1,1),
	UserId INT NOT NULL,
    LoginDate DATE NOT NULL,
    LoginTime TIME NOT NULL,
	LogoutDate DATE NULL,
    LogoutTime TIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE Products (
    Product_Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Product_Name VARCHAR(255) NOT NULL,
    Product_Price BIGINT NOT NULL,
    Product_Count BIGINT NOT NULL,
);



Delete from Users WHERE Id = '1';


SELECT * FROM Users;

SELECT * FROM UserLogins;

SELECT * FROM Products;


DROP TABLE UserLogins;

DROP TABLE Products;

DROP TABLE Users;


SELECT 
    Users.Id AS UserId,
    Users.Username,
    Users.Email,
    Users.Mobile_No,
    Users.UserPassword,
    Users.RegistrationDate,
    Users.RegistrationTime,
    Users.UserRole,
    UserLogins.Id AS LoginId,
    UserLogins.LoginDate,
    UserLogins.LoginTime
FROM Users
LEFT JOIN UserLogins ON Users.Id = UserLogins.UserId;


DELETE FROM UserLogins WHERE Id = 1;