/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.36.0
*/

ALTER TABLE [dbo].[RETAILTERMINALTABLE]
ADD
	[OPENDRAWERONCLOSESHIFT] [int] NOT NULL DEFAULT((0))
GO