CREATE TABLE dbo.TemplateHistory (
    ID char(32) NOT NULL,
    TemplateId char(32) NOT NULL,
    UserId char(32) NOT NULL,
    DateUsed datetime NOT NULL,
    GeneratedLink nvarchar(max) NOT NULL,
    CONSTRAINT PK_TemplateHistory_ID PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_TemplateHistory_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User](ID),
    CONSTRAINT FK_TemplateHistory_TemplateID FOREIGN KEY (TemplateId) REFERENCES dbo.[Template](ID)  
);