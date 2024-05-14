SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [farm].[Farms](
                               [id] [uniqueidentifier] NOT NULL,
                               [userId] [nvarchar](255) NOT NULL,
                               [crop] [nvarchar](255) NOT NULL,
                               [location] [nvarchar](255) NOT NULL,
                               [size] [decimal] NOT NULL,
                               [coordinates] [nvarchar](255) NULL,
                               [budget] [decimal] NOT NULL,
                               [startDateTime] [datetimeoffset](7) NOT NULL,
                               [estimatedCost] [decimal] NOT NULL,
                               [estimatedRevenue] [decimal] NOT NULL,
                               [harvestInDays] [int] NOT NULL,
                               [createdDateTime] [datetimeoffset](7) NOT NULL,
                               [updatedDateTime] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [farm].[Farms] ADD PRIMARY KEY CLUSTERED
    (
     [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [farm].[Farms] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [farm].[Farms] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [farm].[Farms] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
