CREATE TABLE Template
(
	ID char(32) NOT NULL,
	UserId char(32) NOT NULL,
	DocumentLink NVARCHAR(max) NOT NULL,
	Title NVARCHAR(100) NOT NULL,
	PreviewImageLink NVARCHAR(max) NOT NULL,
    UploadDate datetime NOT NULL,
    Keywords NVARCHAR(max) NOT NULL,
    Description NVARCHAR(max) NOT NULL,
	CONSTRAINT PK_Template_ID PRIMARY KEY CLUSTERED (ID ASC),
	CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES dbo.[User](ID)
)