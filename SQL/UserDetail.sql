CREATE TABLE dbo.UserDetail (
	ID int NOT NULL IDENTITY(1,1),
	UserId int NOT NULL,
	DetailType nvarchar(40) NOT NULL,
	FromDate datetime,
	ToDate datetime,
	DetailText nvarchar(max) NOT NULL,
);

ALTER TABLE dbo.UserDetail ADD CONSTRAINT PK_UserDetail_ID PRIMARY KEY (ID);
ALTER TABLE dbo.UserDetail ADD CONSTRAINT FK_UserDetail_UserId FOREIGN KEY (ID) REFERENCES dbo.[User](ID);