CREATE TABLE dbo.[User]
(
    ID char(32) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    AvailableCash float NOT NULL,
    VerifyString char(32) NOT NULL,
    Verified bit NOT NULL,
    CONSTRAINT [PK_User_UserID] PRIMARY KEY CLUSTERED (ID ASC)
)