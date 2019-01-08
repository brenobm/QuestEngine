IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [PlayerProgresses] (
    [PlayerId] nvarchar(50) NOT NULL,
    [QuestId] int NOT NULL,
    [QuestPointsEarned] bigint NOT NULL,
    [LastMilestoneCompletedId] int NULL,
    CONSTRAINT [PK_PlayerProgresses] PRIMARY KEY ([PlayerId], [QuestId])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190108145342_InitialMigration', N'2.2.0-rtm-35687');

GO

