SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [farm].[FarmDailyTask](
                                   [id] [uniqueidentifier] NOT NULL,
                                   [farmDailyId] [uniqueidentifier] NOT NULL,
                                   [day] [int] NOT NULL,
                                   [description] [nvarchar](255) NOT NULL,
                                   [isCompleted] [bit] NOT NULL DEFAULT 0,
                                   [createdDateTime] [datetimeoffset](7) NOT NULL,
                                   [updatedDateTime] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [farm].[FarmDailyTask] ADD PRIMARY KEY CLUSTERED
    (
     [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [farm].[FarmDailyTask] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [farm].[FarmDailyTask] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [farm].[FarmDailyTask] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [farm].[FarmDailyTask]  WITH CHECK ADD  CONSTRAINT [FK_FarmDailyTask_FarmDailyId] FOREIGN KEY([farmDailyId])
    REFERENCES [farm].[FarmDaily] ([Id])
GO
ALTER TABLE [farm].[FarmDailyTask] CHECK CONSTRAINT [FK_FarmDailyTask_FarmDailyId]
GO
ALTER TABLE [farm].[FarmDailyTask] DROP CONSTRAINT [FK_FarmDailyTask_FarmDailyId]
GO
ALTER TABLE [farm].[FarmDailyTask] ADD farmId [uniqueidentifier], FOREIGN KEY(farmId) REFERENCES [farm].[Farms](id)
GO
