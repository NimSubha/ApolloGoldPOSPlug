/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.1.0
*/

ALTER TABLE [dbo].[RETAILINVENTORYHEADER_BR]
ADD
	[FISCALPRINTERSERIALNUMBER] [nvarchar](20) NOT NULL DEFAULT ('')
GO
