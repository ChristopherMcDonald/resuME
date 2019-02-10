DROP table dbo.[SkillDetail];
DROP table dbo.[CertDetail];
DROP table dbo.[EducationDetail];
DROP table dbo.[WorkDetail];
DROP table dbo.[ProjectDetail];

DROP table dbo.[RevenueHistory];
DROP table dbo.[TemplateHistory];
DROP table dbo.[Template];
DROP table dbo.[Favourite];
DROP table dbo.[User];

CREATE TABLE dbo.[User]
(
    ID uniqueidentifier NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    AvailableCash float NOT NULL,
    VerifyString uniqueidentifier NOT NULL,
    Verified bit NOT NULL,
    CONSTRAINT [PK_User_UserID] PRIMARY KEY CLUSTERED (ID ASC)
)

CREATE TABLE Template
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    DocumentLink NVARCHAR(max) NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    PreviewImageLink NVARCHAR(max) NOT NULL,
    UploadDate datetime NOT NULL,
    Keywords NVARCHAR(max) NOT NULL,
    Description NVARCHAR(max) NOT NULL,
    CONSTRAINT PK_Template_ID PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES dbo.[User](ID)
)

CREATE TABLE dbo.Favourite (
    UserId uniqueidentifier NOT NULL,
    TemplateID uniqueidentifier NOT NULL
)

CREATE TABLE dbo.RevenueHistory (
	ID uniqueidentifier NOT NULL,
	UserId uniqueidentifier NOT NULL,
	Date datetime NOT NULL,
	Action nvarchar(40) NOT NULL,
	Amount float NOT NULL,
	TemplateUseId int
);

-- no Foreign Key for TemplateUseId

ALTER TABLE dbo.RevenueHistory ADD CONSTRAINT PK_RevenueHistory_ID PRIMARY KEY (ID);
ALTER TABLE dbo.RevenueHistory ADD CONSTRAINT FK_RevenueHistory_UserId FOREIGN KEY (UserId) REFERENCES dbo.[User](ID);

CREATE TABLE dbo.[WorkDetail]
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    StartDate datetime NOT NULL,
    EndDate datetime NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Location NVARCHAR(100) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    Detail NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_WorkDetail_ID] PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_WorkDetail_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User] (ID) ON DELETE CASCADE ON UPDATE CASCADE  
);


CREATE TABLE dbo.[ProjectDetail]
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    StartDate datetime NOT NULL,
    EndDate datetime NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Supervisor NVARCHAR(100) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    Detail NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_ProjectDetail_ID] PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_ProjectDetail_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User] (ID) ON DELETE CASCADE ON UPDATE CASCADE  
);

CREATE TABLE dbo.[EducationDetail]
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    StartDate datetime NOT NULL,
    EndDate datetime NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Degree NVARCHAR(100) NOT NULL,
    Achievements NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_EducationSkill_ID] PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_EducationSkill_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User] (ID) ON DELETE CASCADE ON UPDATE CASCADE  
);

CREATE TABLE dbo.[CertDetail]
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    DateAchieved datetime NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Issuer NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_CertDetail_ID] PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_CertDetail_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User] (ID) ON DELETE CASCADE ON UPDATE CASCADE  
);

CREATE TABLE dbo.[SkillDetail]
(
    ID uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    Level NVARCHAR(100) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Class NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_SkillDetail_ID] PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_SkillDetail_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User] (ID) ON DELETE CASCADE ON UPDATE CASCADE  
);

CREATE TABLE dbo.TemplateHistory (
    ID uniqueidentifier NOT NULL,
    TemplateId uniqueidentifier NOT NULL,
    UserId uniqueidentifier NOT NULL,
    DateUsed datetime NOT NULL,
    GeneratedLink nvarchar(max) NOT NULL,
    CONSTRAINT PK_TemplateHistory_ID PRIMARY KEY CLUSTERED (ID ASC),
    CONSTRAINT FK_TemplateHistory_UserID FOREIGN KEY (UserId) REFERENCES dbo.[User](ID),
    CONSTRAINT FK_TemplateHistory_TemplateID FOREIGN KEY (TemplateId) REFERENCES dbo.[Template](ID)  
);