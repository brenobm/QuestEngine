# Quest Engine

## How to Run it
Pre-requirements:
* dot.Net Core 2.2 SDK
* SQL Server Instance

Configure SQL Server connection string on the file /QuestEngine/appsettings.json

Restore the NuGet Packages:
```
dotnet restore QuestEngine.sln
```

Build the application:
```
dotnet build QuestEngine.sln
```

Run the migrations to create the database:
```
dotnet ef database update 
```

Run the Web Application
```
dotnet run --project QuestEngine/QuestEngine.csproj
```

And access from the browser the URL => http://localhost:5000/swagger/index.html

## About the application
The Quest Engine as build upon ASP.Net Core 2.2 with the Swagger API interface, that allows testing all available endpoints.

The API was designed using Swagger Specification on the file [questengine.json](QuestEngine/swagger/questengine.json) and auto generate the Controllers and Clients by [NSwag Studio](https://github.com/RSuter/NSwag/wiki/NSwagStudio).

For persistence, it is using Entity Framework Core 2.2. EF Core has the "In Memory" database feature that allows to run tests without any database setup.

To enable it, just change the InMemory configuration on appsettings.json to *true*.
However, in the application settings, it is possible to disable the "In Memory" feature and define a Microsoft SQL Server connection string to create the database.  All migrations are enabled.

## Unit tests

For running the unit tests just run the command:
```
dotnet test
```

## Quest configurations

The configuration about the quest is on the appsettings.json file, with the following properties:

* GameConfiguration - Hold the main level configurations about the quest

  * RateFromBet - Rate to multiply the amount bet when calculating the Quest Point Earned

  * LevelBonusRate - Rate to multiply the player level when calculating the Quest Point Earned

  * TotalQuestPoints - Total points needed to finish the Quest

  * QuestId - To identify the current quest. It allows saving more than one quest on the database for the same player.

* MilestoneConfiguration - Hold all the milestones in the quest. It is an array of milestones.

  * MilestoneId - The ID of the milestone

  * ChipsAwarded - Amount of chips that the player will award when he archives the milestone

  * MilestoneQuestPoints - The number of quest points that the player should get to complete this milestone.


## Database schema

The database consists of one table: *PlayerProgress*

This table has the following fields:

* PlayerId - varchar(50) - Id of the player

* QuestId - int - Id of the quest

* QuestPointsEarned - bigint - Total of points earned by the player in the current quest

* LastMilestoneCompletedId - int - Last Milestone ID completed by the player

With the composed primary key {PlayerId, QuestId}


```
CREATE TABLE [PlayerProgresses] (
    [PlayerId] nvarchar(50) NOT NULL,
    [QuestId] int NOT NULL,
    [QuestPointsEarned] bigint NOT NULL,
    [LastMilestoneCompletedId] int NULL,
    CONSTRAINT [PK_PlayerProgresses] PRIMARY KEY ([PlayerId], [QuestId])
);
```

## Sequence Diagram

In the picture below is showed the sequence diagram about the request to response execution.
![Sequence Diagram](/ProgressSequenceDiagram.png "Sequence Diagram").

It can be viewed on file /ProgressSequenceDiagram.png