/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.14.0
*/

/****** adding new column to [RETAILPARAMETERS] to enforce return in later shift control for Russia ******/
ALTER TABLE [dbo].[RETAILPARAMETERS]
ADD 
	/*default it to one, in order to maintain original behavior when POS part is updated and AX part is not*/
	[PROCESSRETURNSASINORIGINALSALESHIFT_RU] [int] NOT NULL DEFAULT (1)
GO

/****** adding new column to [RETAILTRANSACTIONPAYMENTTRANS] to have the number of cash disbursement slip for cash returns in later shift for Russia ******/
ALTER TABLE [dbo].[RETAILTRANSACTIONPAYMENTTRANS]
ADD
	[CASHDOCID_RU] [NVARCHAR](10) NOT NULL DEFAULT('')
GO

/****** adding new column to [RETAILTRANSACTIONTABLE] to be able to skip aggregation (regardless of the settings in the parameters) when posting statement ******/
ALTER TABLE [dbo].[RETAILTRANSACTIONTABLE]
ADD
	[SKIPAGGREGATION] [int] NOT NULL DEFAULT(0)
GO