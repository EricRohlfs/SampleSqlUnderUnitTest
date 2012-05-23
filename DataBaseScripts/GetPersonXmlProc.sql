USE [XmlTest]
GO

/****** Object:  StoredProcedure [dbo].[GetPersonXml]    Script Date: 05/23/2012 15:58:51 ******/
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

