USE [master]
GO
/****** Object:  Database [XmlTest]    Script Date: 05/23/2012 16:00:40 ******/
CREATE DATABASE [XmlTest] ON  PRIMARY 
( NAME = N'XmlTest', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS08\MSSQL\DATA\XmlTest.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'XmlTest_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS08\MSSQL\DATA\XmlTest_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [XmlTest] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [XmlTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [XmlTest] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [XmlTest] SET ANSI_NULLS OFF
GO
ALTER DATABASE [XmlTest] SET ANSI_PADDING OFF
GO
ALTER DATABASE [XmlTest] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [XmlTest] SET ARITHABORT OFF
GO
ALTER DATABASE [XmlTest] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [XmlTest] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [XmlTest] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [XmlTest] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [XmlTest] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [XmlTest] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [XmlTest] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [XmlTest] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [XmlTest] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [XmlTest] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [XmlTest] SET  DISABLE_BROKER
GO
ALTER DATABASE [XmlTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [XmlTest] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [XmlTest] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [XmlTest] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [XmlTest] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [XmlTest] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [XmlTest] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [XmlTest] SET  READ_WRITE
GO
ALTER DATABASE [XmlTest] SET RECOVERY SIMPLE
GO
ALTER DATABASE [XmlTest] SET  MULTI_USER
GO
ALTER DATABASE [XmlTest] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [XmlTest] SET DB_CHAINING OFF
GO
USE [XmlTest]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 05/23/2012 16:00:40 ******/
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
/****** Object:  Table [dbo].[Person]    Script Date: 05/23/2012 16:00:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Person](
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[GetPersonXml]    Script Date: 05/23/2012 16:00:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPersonXml]
 @personId  int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT P.PersonId, P.FirstName, P.LastName
, Addresses = (
	select A.AddressId, A.Address1, A.City, A.State 
	From dbo.Address as A
	Where A.PersonId = P.PersonId
	For Xml Path('Address'), Type, Elements
	) 
FROM dbo.Person  as P 
  
FOR XML Path('Person'), Type, Elements

END
GO
