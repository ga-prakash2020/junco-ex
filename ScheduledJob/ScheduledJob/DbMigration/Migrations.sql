CREATE DATABASE [TESTDB]

GO

USE [TestDB]
GO

/****** Object:  Table [dbo].[Notifications]    Script Date: 05-05-2020 09:06:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Notifications](
	[Id] [int] NOT NULL,
	[Email] [varchar](60) NOT NULL,
	[NotifiedOn] [datetime] NULL,
	[IsViewed] [bit] NOT NULL,
	[IsThankYouSent] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((0)) FOR [IsViewed]
GO

ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((0)) FOR [IsThankYouSent]
GO
