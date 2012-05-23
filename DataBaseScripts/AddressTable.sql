USE [XmlTest]
GO

/****** Object:  Table [dbo].[Address]    Script Date: 05/23/2012 15:58:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Address](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[Address1] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

