CREATE TABLE dbo.RevenueHistory (
	ID int NOT NULL IDENTITY(1,1),
	UserId int NOT NULL,
	Date datetime NOT NULL,
	Action nvarchar(40) NOT NULL,
	Amount float NOT NULL,
	TemplateUseId int
);

-- no Foreign Key for TemplateUseId

ALTER TABLE dbo.RevenueHistory ADD CONSTRAINT PK_RevenueHistory_ID PRIMARY KEY (ID);
ALTER TABLE dbo.RevenueHistory ADD CONSTRAINT FK_RevenueHistory_UserId FOREIGN KEY (UserId) REFERENCES dbo.[User](ID);