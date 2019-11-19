/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.24.0
*/


ALTER TABLE [dbo].[RETAILVISUALPROFILE]
ADD
	[FONTSCHEME] [int] NOT NULL DEFAULT (0)
GO
