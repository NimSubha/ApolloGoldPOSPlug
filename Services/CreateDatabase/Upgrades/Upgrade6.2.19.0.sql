/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.19.0
*/

/****** adding new column to [RETAILTRANSACTIONTABLE] ******/
ALTER TABLE [dbo].[RETAILTRANSACTIONTABLE]
ADD 
	[LOYALTYDISCAMOUNT_RU] [numeric](32, 16) NOT NULL DEFAULT ((0))
GO

/****** adding new column to [RETAILTRANSACTIONSALESTRANS] ******/
ALTER TABLE [dbo].[RETAILTRANSACTIONSALESTRANS]
ADD 
	[LOYALTYDISCAMOUNT_RU] [numeric](32, 16) NOT NULL DEFAULT ((0)),
	[LOYALTYDISCPCT_RU] [numeric](32, 16) NOT NULL DEFAULT ((0))
GO

/****** adding new column to [RETAILPARAMETERS] ******/
ALTER TABLE [dbo].[RETAILPARAMETERS]
ADD 
	[AUTOMATICRETURNOFLOYALTYPAYMENT_RU] [int] NOT NULL DEFAULT ((0))
GO