/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.17.0
*/

/****** adding new column to [RETAILPARAMETERS] for Russia ******/
ALTER TABLE [dbo].[RETAILPARAMETERS]
ADD 
	[PROCESSGIFTCARDSASPREPAYMENTS_RU] [int] NOT NULL DEFAULT ((0))
GO
