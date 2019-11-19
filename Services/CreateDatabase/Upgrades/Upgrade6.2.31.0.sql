/*
    Microsoft Dynamics AX for Retail POS Upgrade Database Script
    DbVersion: 6.2.31.0
*/

ALTER TABLE [dbo].[RETAILTRANSACTIONSALESTRANS]
ADD 	
	[ORIGINALPRICE] [numeric](32, 16) NOT NULL  DEFAULT ((0))
GO

