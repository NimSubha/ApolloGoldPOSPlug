/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.35.0
*/

/****** adding new column to [RETAILSTORETABLE] to enforce return in later shift control for ******/
ALTER TABLE [dbo].[RETAILSTORETABLE]
ADD 
	/*default it to zero, in order to maintain original behavior as a zero amount entered means
	 the payment method wasn't counted, and the previously entered amount was used*/
	[ZEROTENDERDECLARATION] [int] NOT NULL DEFAULT (0)
GO