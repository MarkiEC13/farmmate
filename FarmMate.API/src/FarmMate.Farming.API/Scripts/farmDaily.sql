SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [farm].[FarmDaily](
                               [id] [uniqueidentifier] NOT NULL,
                               [farmId] [uniqueidentifier] NOT NULL,
                               [day] [int] NOT NULL,
                               [description] [nvarchar](255),
                               [date] [datetimeoffset](7) NOT NULL,
                               [imageUrl] [nvarchar](255),
                               [createdDateTime] [datetimeoffset](7) NOT NULL,
                               [updatedDateTime] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [farm].[FarmDaily] ADD PRIMARY KEY CLUSTERED
    (
     [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [farm].[FarmDaily] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [farm].[FarmDaily] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [farm].[FarmDaily] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [farm].[FarmDaily]  WITH CHECK ADD  CONSTRAINT [FK_FarmDaily_FarmId] FOREIGN KEY([farmId])
    REFERENCES [farm].[Farms] ([Id])
GO
ALTER TABLE [farm].[FarmDaily] CHECK CONSTRAINT [FK_FarmDaily_FarmId]
GO
