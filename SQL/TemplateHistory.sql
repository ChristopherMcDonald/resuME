CREATE TABLE dbo.TemplateHistory (
	ID int NOT NULL IDENTITY(1,1),
	TemplateId int NOT NULL,
	UserId int NOT NULL,
	DateUsed datetime NOT NULL
);

ALTER TABLE dbo.TemplateHistory ADD CONSTRAINT PK_TemplateHistory_ID PRIMARY KEY (ID);
ALTER TABLE dbo.TemplateHistory ADD CONSTRAINT FK_TemplateHistory_TemplateId FOREIGN KEY (TemplateId) REFERENCES dbo.[Template](ID);
ALTER TABLE dbo.TemplateHistory ADD CONSTRAINT FK_TemplateHistory_UserId FOREIGN KEY (UserId) REFERENCES dbo.[User](ID);