/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.6.0
*/

ALTER TABLE [dbo].[RETAILFUNCTIONALITYPROFILE]
ADD
	[APPLYDISCOUNTONUNITPRICES] [int] NOT NULL DEFAULT((0))
GO
