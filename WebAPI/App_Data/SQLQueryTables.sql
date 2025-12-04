DROP TABLE dbo.Applications;
DROP TABLE dbo.Containers;
DROP TABLE dbo.ContentInstances;
DROP TABLE dbo.Subscriptions;


CREATE TABLE [dbo].[Applications]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ResourceName] NVARCHAR(200) NOT NULL,
    [CreationDateTime] DATETIME2 NOT NULL
)
CREATE TABLE [dbo].[Containers]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ResourceName] NVARCHAR(200) NOT NULL,
    [CreationDateTime] DATETIME2 NOT NULL,
    [ApplicationId] INT NOT NULL,

    CONSTRAINT [FK_Containers_Applications]
        FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications]([Id])
)
CREATE TABLE [dbo].[ContentInstances]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ResourceName] NVARCHAR(200) NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [CreationDateTime] DATETIME2 NOT NULL,
    [ContainerId] INT NOT NULL,

    CONSTRAINT [FK_ContentInstances_Containers]
        FOREIGN KEY ([ContainerId]) REFERENCES [dbo].[Containers]([Id])
)
CREATE TABLE [dbo].[Subscriptions]
(
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ResourceName] NVARCHAR(200) NOT NULL,
    [Evt] INT NOT NULL,
    [Endpoint] NVARCHAR(500) NOT NULL,
    [CreationDateTime] DATETIME2 NOT NULL,
    [ContainerId] INT NOT NULL,

    CONSTRAINT [FK_Subscriptions_Containers]
        FOREIGN KEY ([ContainerId]) REFERENCES [dbo].[Containers]([Id])
)
