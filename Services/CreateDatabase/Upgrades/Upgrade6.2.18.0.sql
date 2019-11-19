/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.18.0
*/

/****** adding new column to [RETAILTERMINALTABLE] for Russia ******/
ALTER TABLE [dbo].[RETAILTERMINALTABLE]
ADD 
	[EFTTENDERTYPEIDDEFAULT] [nvarchar](10) NOT NULL DEFAULT ('')
GO

/****** adding new column to [RETAILFUNCTIONALITYPROFILE] for Russia ******/
ALTER TABLE [dbo].[RETAILFUNCTIONALITYPROFILE]
ADD 
	[EODBANKTOTALSVERIFICATION] [int] NOT NULL DEFAULT (0)
GO